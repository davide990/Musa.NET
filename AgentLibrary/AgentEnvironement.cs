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
        /// The moment in which this environment has been created.
        /// </summary>
        public DateTime CreationDate
        {
            get { return creationTime; }
            private set { creationTime = value; }
        }
        private DateTime creationTime;

        /// <summary>
        /// Count how many much time elapsed between the moment in which this environment 
        /// has been created and now.
        /// </summary>
        public TimeSpan UpTime
        {
            get { return DateTime.Now.Subtract(CreationDate); }
        }

        /// <summary>
        /// Return the IP address of the machine in which this environment is located
        /// </summary>
        public string IPAddress
        {
            get { return Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString(); }
        }
        
        #endregion Properties

        private static AgentEnvironement instance;

        /// <summary>
        /// Get the unique agent environement for this MUSA.NET process instance.
        /// </summary>
        /// <param name="port"></param>
        /// <param name="ip_address"></param>
        /// <returns></returns>
        public static AgentEnvironement GetInstance(string port = "8080", string ip_address = "localhost")
        {
            if(instance == null)
            {
                instance = new AgentEnvironement();
                EnvironmentServer srv = new EnvironmentServer(instance);

                //TODO QUESTE INFORMAZIONI DOVREBBERO POTER 
                //ESSERE PASSATE DAL CODICE O DA FILE DI CONFIGURAZIONE
                //if(configFile)
                // port <- parseConfigFile(Port)
                // ip_address <- parseConfigFile(IP)

                srv.StartNetworking(port, ip_address);
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

            CreationDate = DateTime.Now;

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
        
        #region Collection delegates
        
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

        #endregion

        /// <summary>
        /// Register an agent to this environment
        /// </summary>
        public void RegisterAgent(Agent a)
        {
            if (!registeredAgents.Contains(a))
                registeredAgents.Add(a);
        }
        
        /// <summary>
		/// Add a statement (as atomic formula) into this environment. The addition of the formula(s) is notified to 
		/// every registered agent (to this environment).
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

                    //get the value of the current term
                    object name = Convert.ChangeType(variableTermType.GetProperty("Name").GetValue(varTerm), typeof(string));
                    object value = variableTermType.GetProperty("Value").GetValue(varTerm);
                    value = Convert.ChangeType(value, varTerm.GetType().GetGenericArguments()[0]);

                    attributes.Add(AssignmentType.CreateAssignmentForTerm((string)name, value, varTerm.GetType().GetGenericArguments()[0]));
                }

                //Add the formula to this environment
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
