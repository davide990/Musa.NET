﻿using AgentLibrary.Networking;
using FormulaLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading;
using MusaLogger;
using MusaConfiguration;

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

		public ObservableCollection<Agent> RegisteredAgents {
			get { return registeredAgents; }
			set {
				if (value != null)
					registeredAgents = value;
			}
		}

		private static ManualResetEvent mre = new ManualResetEvent (false);

		#endregion Fields

		#region Properties

		/// <summary>
		/// The moment in which this environment has been created.
		/// </summary>
		public DateTime CreationDate {
			get { return creationTime; }
			private set { creationTime = value; }
		}

		private DateTime creationTime;

		/// <summary>
		/// Count how many much time elapsed between the moment in which this environment 
		/// has been created and now.
		/// </summary>
		public TimeSpan UpTime {
			get { return DateTime.Now.Subtract (CreationDate); }
		}

		/// <summary>
		/// Return the IP address of the machine in which this environment is located
		/// </summary>
		public string IPAddress {
			get { return Dns.GetHostEntry (Dns.GetHostName ()).AddressList [0].ToString (); }
		}

		#endregion Properties

		private static AgentEnvironement instance;

		/// <summary>
		/// The logger this environment uses.
		/// </summary>
		private LoggerSet logger;

		/// <summary>
		/// Get the unique agent environement for this MUSA.NET process instance.
		/// </summary>
		public static AgentEnvironement GetInstance ()
		{
			if (instance == null) 
			{
				instance = new AgentEnvironement ();
                
				if (MusaConfig.GetConfig ().NetworkingEnabled) 
				{
					int port = MusaConfig.GetConfig ().MusaAddressPort;
					string ip_address = MusaConfig.GetConfig ().MusaAddress;

					EnvironmentServer srv = new EnvironmentServer (instance);
					srv.StartNetworking (port.ToString (), ip_address);
				}

				instance.logger = MusaConfig.GetLoggerSet ();
			}

			return instance;
		}

		#region Constructors

		/// <summary>
		/// Create a new environment
		/// </summary>
		private AgentEnvironement ()
		{
			
			statements = new ObservableCollection<AtomicFormula> ();
			attributes = new ObservableCollection<AssignmentType> ();
			registeredAgents = new ObservableCollection<Agent> ();

			CreationDate = DateTime.Now;

			registeredAgents.CollectionChanged += RegisteredAgents_CollectionChanged;
            
			statements.CollectionChanged += Statements_CollectionChanged;
			attributes.CollectionChanged += Attributes_CollectionChanged;
		}

		private void RegisteredAgents_CollectionChanged (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (RegisteredAgents.Count > 0)
				mre.Reset ();
			else
				mre.Set ();

			switch (e.Action) {
			case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
				logger.Log (LogLevel.Trace, "Added agent(s): " + e.NewItems);
				break;

			case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
				logger.Log (LogLevel.Trace, "Removed agent(s): " + e.NewItems);
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
		private void Attributes_CollectionChanged (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			foreach (Agent a in registeredAgents) {
				if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
					a.notifyEnvironementChanges (PerceptionType.SetBeliefValue, e.NewItems);
				else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
					a.notifyEnvironementChanges (PerceptionType.UnSetBeliefValue, e.OldItems);
				else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
					a.notifyEnvironementChanges (PerceptionType.UpdateBeliefValue, e.OldItems);
			}
		}

		/// <summary>
		/// Method invoked when a changes that involves the statements occurs into the environment's statement 
		/// list. It is responsible for communicating the changes to the registered agents.
		/// </summary>
		private void Statements_CollectionChanged (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			foreach (Agent a in registeredAgents) {
				if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
					a.notifyEnvironementChanges (PerceptionType.AddBelief, e.NewItems);
				else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
					a.notifyEnvironementChanges (PerceptionType.RemoveBelief, e.OldItems);
			}
		}

		#endregion

		/// <summary>
		/// Register an agent to this environment
		/// </summary>
		public void RegisterAgent (Agent a)
		{
			if (!registeredAgents.Contains (a))
				registeredAgents.Add (a);
		}

		/// <summary>
		/// Add a statement (as atomic formula) into this environment. The addition of the formula(s) is notified to 
		/// every registered agent (to this environment).
		/// </summary>
		public void RegisterStatement (params AtomicFormula[] f)
		{
			List<object> variableTerms = new List<object> ();
			foreach (AtomicFormula ff in f) {
				variableTerms = ff.ConvertToSimpleFormula ();

				if (statements.Contains (ff))
					continue;

				foreach (object varTerm in variableTerms) {
					//get the type info for the current term
					Type variableTermType = typeof(VariableTerm<>).MakeGenericType (varTerm.GetType ().GetGenericArguments () [0]);

					//get the value of the current term
					object name = Convert.ChangeType (variableTermType.GetProperty ("Name").GetValue (varTerm), typeof(string));
					object value = variableTermType.GetProperty ("Value").GetValue (varTerm);
					value = Convert.ChangeType (value, varTerm.GetType ().GetGenericArguments () [0]);

					attributes.Add (AssignmentType.CreateAssignmentForTerm ((string)name, value, varTerm.GetType ().GetGenericArguments () [0]));
				}

				//Add the formula to this environment
				statements.Add (ff);
			}
		}

		/// <summary>
		/// Given a set of atomic formulas, this method removes the matching formulas from this environment and also its
		/// corresponding assignments.
		/// </summary>
		public void DeleteStatementAndAssignment (params AtomicFormula[] f)
		{
			List<object> variableTerms = new List<object> ();
			foreach (AtomicFormula ff in f) {
				variableTerms = ff.ConvertToSimpleFormula ();

				if (!statements.Contains (ff))
					continue;

				foreach (object varTerm in variableTerms) {
					//get the type info for the current term
					Type variableTermType = typeof(VariableTerm<>).MakeGenericType (varTerm.GetType ().GetGenericArguments () [0]);

					//get the value of the current term
					object name = Convert.ChangeType (variableTermType.GetProperty ("Name").GetValue (varTerm), typeof(string));
					object value = variableTermType.GetProperty ("Value").GetValue (varTerm);
					value = Convert.ChangeType (value, varTerm.GetType ().GetGenericArguments () [0]);

					attributes.Remove (AssignmentType.CreateAssignmentForTerm ((string)name, value, varTerm.GetType ().GetGenericArguments () [0]));
				}

				//Remove the formula from this environment
				statements.Remove (ff);
			}
		}

		/// <summary>
		/// Given a set of atomic formulas, this method removes the matching formulas from this environment.
		/// </summary>
		public void DeleteStatement (params AtomicFormula[] f)
		{
			foreach (AtomicFormula ff in f) {
				//Convert the formula to a simple formula
				ff.ConvertToSimpleFormula ();

				if (!statements.Contains (ff))
					continue;

				//Remove the formula from this environment
				statements.Remove (ff);
			}
		}

		/// <summary>
		/// Check if an agent is registered to this environment
		/// </summary>
		public bool IsAgentRegistered (Agent a)
		{
			return registeredAgents.Contains (a);
		}

		public void SetAccessPolicy ()
		{
			/*
                pubblico
                privato
                solo manager di progetto
                solo se disposti di una chiave
            */
			throw new NotImplementedException ();
		}

		public void SetExternalCommunicationPolicy ()
		{
			/*
                pubblico
                privato
                solo manager di progetto
                solo se disposti di una chiave
            */
			throw new NotImplementedException ();
		}

		/// <summary>
		/// Block the execution of the main process until at least one agent is registered within this environment.
		/// </summary>
		public void WaitForAgents ()
		{
			mre.WaitOne ();
		}


		#endregion

	}
}
