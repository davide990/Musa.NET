﻿using AgentLibrary.Networking;
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
        /// The statements for this environement
        /// </summary>
        private ObservableCollection<AtomicFormula> statements;

        /// <summary>
        /// The set of attributes for the statement contained in this environement
        /// </summary>
        private ObservableCollection<AssignmentType> attributes;

        /// <summary>
        /// The agents registered to this environement
        /// </summary>
        private HashSet<Agent> registeredAgents;

        #endregion Fields

        #region Properties

        /// <summary>
        /// The communication service interface used by this environement. This object act like
        /// a bridge for communication between agents located in different (or within the same) 
        /// environement. Every communication of agents is forwarded to an environement that will
        /// forward to the receiver agent, agent group or environement (depending on the communication
        /// type)
        /// </summary>
        private MusaCommunicationService CommunicationService
        {
            get { return MusaCommunicationService.getInstance(); }
        }

        /// <summary>
        /// Return the IP address of the machine in which this environement is located
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

        #region Events

        public EventHandler onNetworkServiceStart = null;

        #endregion
        
        #region Constructors

        /// <summary>
        /// Create a new environement
        /// </summary>
        public AgentEnvironement()
        {
            statements = new ObservableCollection<AtomicFormula>();
            attributes = new ObservableCollection<AssignmentType>();
            registeredAgents = new HashSet<Agent>();

            statements.CollectionChanged += Statements_CollectionChanged;
            attributes.CollectionChanged += Attributes_CollectionChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="local_ip_address"></param>
        public AgentEnvironement(string port, string local_ip_address = "localhost")
        {
            statements = new ObservableCollection<AtomicFormula>();
            attributes = new ObservableCollection<AssignmentType>();
            registeredAgents = new HashSet<Agent>();

            statements.CollectionChanged += Statements_CollectionChanged;
            attributes.CollectionChanged += Attributes_CollectionChanged;

            StartNetworking(port, local_ip_address);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Setup and start the networking service for this environement
        /// </summary>
        /// <param name="address">the IP address of the machine in which this environement is located</param>
        /// <param name="port">the port used by this environement</param>
        public void StartNetworking(string port, string local_ip_address = "localhost")
        {
            //var address = new Uri("http://localhost:8080");
            Uri address = new Uri("http://" + local_ip_address + ":" + port);
            ServiceHost host = new ServiceHost(typeof(MusaCommunicationService));
            host.AddServiceEndpoint(typeof(IMusaCommunicationService), new BasicHttpBinding(), address);
            host.Open();

            if (onNetworkServiceStart != null)
                onNetworkServiceStart.Invoke(this, null);
        }

        /// <summary>
        /// Stop the networking service for this environement
        /// </summary>
        public void StopNetworkingService()
        {
            host.Close();
        }


        /// <summary>
        /// Method invoked when a changes that involves the attributes occurs into the environement's statement 
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
        /// Method invoked when a changes that involves the statements occurs into the environement's statement 
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
        /// Register an agent to this environement
        /// </summary>
        public void RegisterAgent(Agent a)
        {
            if (!registeredAgents.Contains(a))
                registeredAgents.Add(a);
        }

        /// <summary>
        /// Register an agent to this environement
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
        /// Check if an agent is registered to this environement
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
            throw new System.NotImplementedException();
        }

        public void SetExternalCommunicationPolicy()
        {
            /*
                pubblico
                privato
                solo manager di progetto
                solo se disposti di una chiave
            */
            throw new System.NotImplementedException();
        }

        #endregion

    }
}
