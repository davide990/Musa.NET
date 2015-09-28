using AgentLibrary.Networking;
using FormulaLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Reflection;
using System.ServiceModel;

namespace AgentLibrary
{
    public sealed class AgentEnvironement
    {
        #region Fields

        /// <summary>
        /// The statements for this environment
        /// </summary>
        private ObservableCollection<AtomicFormula> statements;

        /// <summary>
        /// The set of attributes for the statement contained in this environment
        /// </summary>
        private ObservableCollection<AssignmentType> attributes;

        /// <summary>
        /// The agents registered to this environment
        /// </summary>
        private ObservableCollection<Agent> registeredAgents;

        public ObservableCollection<Agent> RegisteredAgents
        {
            get { return registeredAgents; }
            set { if (value != null) registeredAgents = value; }
        }

        #endregion Fields

        #region Properties
        
        /// <summary>
        /// Return the IP address of the machine in which this environment is located
        /// </summary>
        public string IPAddress
        {
            get { return Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ServiceHost Host
        {
            get { return host; }
            set { host = value; }
        }
        private ServiceHost host;

        #endregion Properties



        private static AgentEnvironement instance;

        public static AgentEnvironement getInstance()
        {
            if(instance == null)
            {
                instance = new AgentEnvironement();
                EnvironmentServer srv = new EnvironmentServer(instance);

                //TODO QUESTE INFORMAZIONI DOVREBBERO POTER 
                //ESSERE PASSATE DAL CODICE O DA FILE DI CONFIGURAZIONE
                srv.StartNetworking("8080");
            }

            return instance;
        }
        
        #region Constructors

        /// <summary>
        /// Create a new environment
        /// </summary>
        private AgentEnvironement()
        {
            statements = new ObservableCollection<AtomicFormula>();
            attributes = new ObservableCollection<AssignmentType>();
            registeredAgents = new ObservableCollection<Agent>();

            registeredAgents.CollectionChanged += RegisteredAgents_CollectionChanged;
            
            statements.CollectionChanged += Statements_CollectionChanged;
            attributes.CollectionChanged += Attributes_CollectionChanged;
        }

        private void RegisteredAgents_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch(e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    Console.WriteLine("Added agent");
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    Console.WriteLine("removed agent");
                    break;
            }
        }
        
        #endregion

        #region Methods
        
        /// <summary>
        /// Method invoked when a changes that involves the attributes occurs into the environment's statement 
        /// list. It is responsible for communicating the changes to the registered agents.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Attributes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (Agent a in registeredAgents)
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                    a.notifyEnvironementChanges(PerceptionType.SetBeliefValue, e.NewItems);
                else
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                    a.notifyEnvironementChanges(PerceptionType.UnSetBeliefValue, e.OldItems);
                else
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                    a.notifyEnvironementChanges(PerceptionType.UpdateBeliefValue, e.OldItems);
            }
        }

        /// <summary>
        /// Method invoked when a changes that involves the statements occurs into the environment's statement 
        /// list. It is responsible for communicating the changes to the registered agents.
        /// </summary>
        private void Statements_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (Agent a in registeredAgents)
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                    a.notifyEnvironementChanges(PerceptionType.AddBelief, e.NewItems);
                else
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                    a.notifyEnvironementChanges(PerceptionType.RemoveBelief, e.OldItems);
            }
        }

        /// <summary>
        /// Register an agent to this environment
        /// </summary>
        public void RegisterAgent(Agent a)
        {
            if (!registeredAgents.Contains(a))
                registeredAgents.Add(a);
        }

        /// <summary>
        /// Register an agent to this environment
        /// </summary>
        public void RegisterAgent(string agent_ip_address)
        {
            //!!!
        }


        /// <summary>
        /// Add a statement (as atomic formula) into this workbench.
        /// </summary>
        public void RegisterStatement(params AtomicFormula[] f)
        {
            List<object> variableTerms = new List<object>();
            foreach (AtomicFormula ff in f)
            {
                variableTerms = ff.ConvertToSimpleFormula();

                if (statements.Contains(ff))
                    continue;

                foreach (object varTerm in variableTerms)
                {
                    //get the type info for the current term
                    Type variableTermType = typeof(VariableTerm<>).MakeGenericType(varTerm.GetType().GetGenericArguments()[0]);
                    ConstructorInfo cinfo = variableTermType.GetConstructor(new[] { typeof(string), varTerm.GetType().GetGenericArguments()[0] });

                    //get the value of the current term
                    object name = Convert.ChangeType(variableTermType.GetProperty("Name").GetValue(varTerm), typeof(string));
                    object value = variableTermType.GetProperty("Value").GetValue(varTerm);
                    value = Convert.ChangeType(value, varTerm.GetType().GetGenericArguments()[0]);

                    attributes.Add(AssignmentType.CreateAssignmentForTerm((string)name, value, varTerm.GetType().GetGenericArguments()[0]));
                }

                //Add the formula to this workbench
                statements.Add(ff);
            }

        }
        
        /// <summary>
        /// Check if an agent is registered to this environment
        /// </summary>
        public bool IsAgentRegistered(Agent a)
        {
            return registeredAgents.Contains(a);
        }

        public void SetAccessPolicy()
        {
            /*
                pubblico
                privato
                solo manager di progetto
                solo se disposti di una chiave
            */
            throw new NotImplementedException();
        }

        public void SetExternalCommunicationPolicy()
        {
            /*
                pubblico
                privato
                solo manager di progetto
                solo se disposti di una chiave
            */
            throw new NotImplementedException();
        }
        
        #endregion

    }
}
