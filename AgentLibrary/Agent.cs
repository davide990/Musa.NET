/**
         __  __                                     _   
        |  \/  |                                   | |  
        | \  / | _   _  ___   __ _     _ __    ___ | |_ 
        | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
        | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
        |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|

*/
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Collections;
using AgentLibrary.Networking;
using PlanLibrary;
using System.Reflection;
using FormulaLibrary;
using FormulaLibrary.ANTLR;


namespace AgentLibrary
{
    public class Agent
    {
        #region Fields

        /// <summary>
        /// The scheduler entity of this agent
        /// </summary>
        private AgentReasoner reasoner;

        /// <summary>
        /// The roles this agent holds
        /// </summary>
        private List<AgentRole> roles;

        /// <summary>
        /// The workbench this agent use to do reasoning activities
        /// </summary>
        private AgentWorkbench workbench;

        /// <summary>
        /// A unique identifier for this agent
        /// </summary>
        private readonly Guid ID;

        /// <summary>
        /// The date in which this agent has been created
        /// </summary>
		public DateTime CreatedAt
		{
			get { return createdAt; }
		}
		private readonly DateTime createdAt;

        /// <summary>
        /// This queue contains the changes to the environment that this agent have to perceive. Since an agent can be 
		/// busy in doing other activities such event-handling or execution of plans, the perceived environment changes 
		/// are accumulated temporarily in this queue until the agent start a perception activity.
		/// The key contains the formula(s) to be perceived (AtomicFormula type).
		/// 
		/// Ogni percezione può coinvolgere più di una singola credenza
        /// </summary>
		public Stack<Tuple<IList, PerceptionType>> PerceivedEnvironementChanges
		{
			get 
			{
				lock (lock_perceivedEnvironmentChanges) 
				{
					return perceivedEnvironementChanges; 
				}
			}
			private set 
			{
				lock (lock_perceivedEnvironmentChanges)
				{
					perceivedEnvironementChanges = value;
				}
			}
		}
		private Stack<Tuple<IList, PerceptionType>> perceivedEnvironementChanges;
        private object lock_perceivedEnvironmentChanges = new object();

        /// <summary>
		/// TODO cambiare la descrizione
		/// 
        /// This stack is the mailbox of the agent. In this structure are collected all the messages that the agent
		/// may receive from other agents in the same, or different, environment. The messages are processed, one by 
		/// one, during the agent's reasoning cycles.
        /// </summary>
		public Stack<Tuple<AgentPassport, AgentMessage>> MailBox
		{
			get { return mailBox; }
			internal set 
			{
				lock(lock_mailBox)
				{
					mailBox = value;
				}
			}
		}
		private Stack<Tuple<AgentPassport, AgentMessage>> mailBox;
        private object lock_mailBox = new object();

		/// <summary>
		/// Gets the plans of this agent.
		/// </summary>
		/// <value>The plans.</value>
		public List<Type> Plans
		{
			get { return new List<Type>(PlansCollection.Keys); }
		}

		/// <summary>
		/// The plans collection.
		/// </summary>
		private Dictionary<Type, IPlanInstance> PlansCollection;

		/// <summary>
		/// Gets a value indicating whether this agent is busy (executing a plan).
		/// </summary>
		public bool Busy
		{
			get { return agent_is_busy; }
			private set {agent_is_busy = value; }
		}
		private bool agent_is_busy;

		/// <summary>
		/// Gets the plan's name that this agent is currently executing.
		/// </summary>
		public IPlanInstance CurrentExecutingPlan
		{
			get
			{
				if (Busy)
					return current_executing_plan;
				else
					return null;
			}

			private set { current_executing_plan = value; }
		}
		private IPlanInstance current_executing_plan;

		/// <summary>
		/// A value indicating wheter this agent must be paused after its current reasoning cycle.
		/// </summary>
		private bool pause_requested;

		/// <summary>
		/// A boolean value used to handle the resumption of this agent's reasoning activity.
		/// </summary>
		private bool resume_reasoning;

        #endregion

		#region Properties

		/// <summary>
		/// Return the IP address of the environment in which this agent is located
		/// </summary>
		public string EnvironementIPAddress
		{
			get { return AgentEnvironement.GetInstance().IPAddress; }
		}

		/// <summary>
		/// The workbench this agent use to do reasoning activities
		/// </summary>
		public AgentWorkbench Workbench
		{
			get { return workbench; }
		}

		/// <summary>
		/// The name  of this agent
		/// </summary>
		public string Name
		{
			get { return name; }
		}
		private readonly string name;

		/// <summary>
		/// Check if this agent is active (in other words, if this agent is doing a reasoning activity)
		/// </summary>
		public bool IsActive
		{
			//get { DateTime now = DateTime.UtcNow; return now >= WorkScheduleStart && now < WorkScheduleEnd; }
			get { return reasoner.IsRunning; }
		}

		/// <summary>
		/// The working end time of this agent
		/// </summary>
		private DateTime _workScheduleEnd;
		public DateTime WorkScheduleEnd
		{
			get { return _workScheduleEnd; }
			private set { _workScheduleEnd = value; }
		}

		/// <summary>
		/// The working start time of this agent
		/// </summary>
		private DateTime _workScheduleStart;
		public DateTime WorkScheduleStart
		{
			get { return _workScheduleStart; }
			private set { _workScheduleStart = value; }
		}

		/// <summary>
		/// The role of this agent
		/// </summary>
		public string Role
		{
			get { return role; }
			set { role = value; }
		}
		private string role;

		/// <summary>
		/// Gets a value indicating whether this agent is paused.
		/// </summary>
		public bool Paused
		{
			get { return _paused; }
			private set { _paused = value; }
		}
		private bool _paused;


		#endregion

        #region Constructors

		/// <summary>
		/// Create a new agent
		/// </summary>
		public Agent () : this ("agent_" + (new Random ().Next (Int32.MaxValue)).ToString())
		{
		}

        /// <summary>
        /// Create a new agent
        /// </summary>
        /// <param name="agent_name"></param>
        public Agent(string agent_name)
        {
			pause_requested = false;
            name = agent_name;
            ID = Guid.NewGuid();
            roles = new List<AgentRole>();
            workbench = new AgentWorkbench(this);
            reasoner = new AgentReasoner(this);
			PerceivedEnvironementChanges = new Stack<Tuple<IList, PerceptionType>>();
			mailBox = new Stack<Tuple<AgentPassport, AgentMessage>> ();// new Dictionary<AgentPassport, AgentMessage>();
			createdAt = DateTime.Now;
			resume_reasoning = false;
			PlansCollection = new Dictionary<Type, IPlanInstance> ();

			Busy = false;
        }

        #endregion

        #region Destructors

        ~Agent()
        {
            //TODO notify agent retirement
            roles.Clear();
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Get the passport of this agent.
        /// </summary>
        /// <returns></returns>
        public AgentPassport GetPassport()
        {
            AgentPassport ps = new AgentPassport();
            ps.AgentName            = Name;
            ps.AgentRole            = Role;
            ps.EnvironementAddress  = EnvironementIPAddress;
            return ps;
        }

        /// <summary>
        /// Notify to this agent a change occurred within the environment this agent is located.
        /// </summary>
        /// <param name="action">The type of change occurred</param>
        /// <param name="changes">The data that involved in the environment change. Each element of [changes] is a 
		/// formula to be added to this agent's workbench</param>
        internal void notifyEnvironementChanges(PerceptionType action, IList changes)
        {
			//TODO log environment change
			foreach (var v in changes)
				Console.WriteLine ("[Agent " + name + "] received " + action.ToString () + " -> " + v.ToString ());
            
			PerceivedEnvironementChanges.Push (new Tuple<IList, PerceptionType> (changes, action));
        }

        /// <summary>
        /// Start this agent
        /// </summary>
        public Agent Start()
        {
            //begin the agent reasoning activity
            reasoner.startReasoning();

            return this;
        }

		/// <summary>
		/// Pause this agent's execution.
		/// </summary>
		public void Pause()
		{
			if (current_executing_plan.IsAtomic ())
			{
				//TODO log agent cannot be paused
				Console.WriteLine ("Agent " + Name + " is executing an atomic plan. It Will be paused after the plan has finished its execution.");

				//Request this agent to pause only after the current atomic plan has finished its execution
				pause_requested = true;
				return;
			}

			//Set the pause state 
			Paused = true;

			//Pause the agent's reasoner
			reasoner.Pause ();

			//Suspend the current executing plan
			CurrentExecutingPlan.Pause ();

			//TODO log agent paused 
			Console.WriteLine ("##PAUSE##");
		}

		/// <summary>
		/// Resume this agent's execution.
		/// </summary>
		public void Resume()
		{
			if (!Paused)
				return;
			
			Paused = false;

			//TODO log resume execution
			Console.WriteLine ("##RESUME##");

			//Resume the execution of the suspended plan
			CurrentExecutingPlan.Resume ();

			if (CurrentExecutingPlan == null)
				reasoner.Resume ();
			else 
			{
				//resume the reasoning activity only after the current plan execution has finished.
				resume_reasoning = true;
			}
		}


		/// <summary>
		/// Adds a plan to this agent.
		/// </summary>
		/// <returns><c>true</c>, if plan was added, <c>false</c> otherwise.</returns>
		/// <param name="plan">The plan model to be added. An instance of this plan is instantiated</param>
		public void AddPlan(Type Plan)
		{
			if (!Plan.BaseType.IsEquivalentTo (typeof(PlanModel)))
				throw new Exception ("Argument #1 in AddPlan(Type) must be of type PlanModel.");
			
			//Create a new plan instance
			Type planInstanceType = typeof(PlanInstance<>).MakeGenericType (Plan);
			var plan_instance = Activator.CreateInstance (planInstanceType);

			//Register the handler for the event 'RegisterResult'
			EventInfo register_result_event = planInstanceType.GetEvent ("RegisterResult");
			MethodInfo delegate_method = GetType ().GetMethod ("onPlanInstanceRegisterResult", BindingFlags.NonPublic|BindingFlags.Instance);
			Delegate register_plan_results_delegate = Delegate.CreateDelegate (register_result_event.EventHandlerType, this, delegate_method);
			register_result_event.AddEventHandler (plan_instance, register_plan_results_delegate);

			//Register the handler for the event 'Finished'
			EventInfo plan_finished_event = planInstanceType.GetEvent ("Finished");
			MethodInfo plan_finished_delegate_method = GetType ().GetMethod ("onPlanInstanceFinished", BindingFlags.NonPublic|BindingFlags.Instance);
			Delegate plan_finished_delegate = Delegate.CreateDelegate (plan_finished_event.EventHandlerType, this, plan_finished_delegate_method);
			plan_finished_event.AddEventHandler (plan_instance, plan_finished_delegate);

			//Add the plan to the agent's plan collection
			PlansCollection.Add (Plan, plan_instance as IPlanInstance);
		}

		#region Agent's plans event handlers

		/// <summary>
		/// Invoked when a plan executed by this agent terminates its execution.
		/// </summary>
		/// <param name="sender">The executed plan.</param>
		/// <param name="args">Arguments.</param>
		private void onPlanInstanceFinished(object sender, EventArgs args)
		{
			//TODO log [agent + plan + event->finished]
			Console.WriteLine ("[Agent " + Name + "] plan " + (sender as IPlanInstance).GetName() + " finished its execution.");

			//Set the value of CurrentExecutingPlan to null
			CurrentExecutingPlan = null;

			//Set the agent busy state to false
			Busy = false;

			//Check if a pause has been requested
			if(pause_requested)
			{
				//Set the pause state 
				Paused = true;

				pause_requested = false;

				//Pause the agent's reasoner
				reasoner.Pause ();

				//TODO log agent paused 
				Console.WriteLine ("##PAUSE##");
				return;
			}

			if (resume_reasoning) 
			{
				reasoner.Resume ();
				resume_reasoning = false;
			}
		}

		/// <summary>
		/// Invoked when a plan requests to register a result.
		/// </summary>
		/// <param name="result">Result.</param>
		private void onPlanInstanceRegisterResult(string result)
		{
			Formula resultFormula = FormulaParser.Parse(result);;

			if(resultFormula != null)
			{
				Workbench.AddStatement(resultFormula);

				//TODO log register result
				Console.WriteLine("[Agent "+Name+"] registered result: "+result);	
			}
		}

		#endregion Agent's plans event handlers

		/// <summary>
		/// Executes a plan.
		/// </summary>
		/// <param name="Plan">Plan.</param>
		/// <param name="args">Arguments.</param>
		internal void ExecutePlan(Type Plan, Dictionary<string, object> args = null)
		{
			if (Busy) 
			{
				throw new Exception ("Agent [" + Name + "] is currently executing plan " + CurrentExecutingPlan);
				return;
			}
				
			//If Plan is not of type PlanModel, then throw an exception
			if (!Plan.BaseType.IsEquivalentTo (typeof(PlanModel)))
				throw new Exception ("Argument #1 in ExecutePlan(Type) must be of type PlanModel.");
			
			Type planInstanceType 	= typeof(PlanInstance<>).MakeGenericType (Plan);
			IPlanInstance the_plan 	= null;

			//Search for the input plan
			PlansCollection.TryGetValue(Plan, out the_plan );

			//If the input plan has not been found within the agent's plans collection, throw an exception
			if (the_plan == null)
				throw new Exception ("Plan '" + Plan.Name + "' not found in agent [" + Name + "] plans collection.");

			//Get the plan's Execute() method
			MethodInfo execute_method = planInstanceType.GetMethod ("Execute", BindingFlags.Public | BindingFlags.Instance);

			//Set the agent to busy
			Busy = true;

			//Set the value for CurrentExecutingPlan 
			CurrentExecutingPlan = the_plan;

			//Execute the plan
			execute_method.Invoke (the_plan, new object[]{ args });

			//TODO [ALTA PRIORITÀ] migliorare il sistema di bloccaggio dell'agente
			//Lock the agent's reasoning life cycle until the invoked plan terminates its execution
			while (Busy);
		}

		/// <summary>
		/// Tell this agent to achieve a goal by executing a specified plan
		/// </summary>
		public void AchieveGoal(Type Plan, Dictionary<string,object> Args = null)
		{
			if (Plan == null) 
				throw new ArgumentNullException ("Plan", "Argument #1 'Plan' cannot be null.");

			//Set the achievement of the plan [Plan] as an agent's perception
			PerceivedEnvironementChanges.Push (new Tuple<IList, PerceptionType> (new List<Type> (){ Plan }, PerceptionType.Achieve));

			//Set the parameters to be used when the plan [Plan] is invoked
			if (Args != null)
				reasoner.EventsArgs.Add (new Tuple<string, PerceptionType> (Plan.Name, PerceptionType.Achieve), Args);
		}

		/// <summary>
		/// Adds an event.
		/// </summary>
		/// <param name="formula">The formula to reason on.</param>
		/// <param name="perception">The perception this event reacts to.</param>
		/// <param name="Plan">The plan to execute.</param>
		/// <param name="Args">The plan to execute.</param>
		public void AddEvent(string formula, PerceptionType perception, Type Plan, Dictionary<string,object> Args = null)
		{
			reasoner.AddEvent (formula, perception, Plan, Args);
		}

		/// <summary>
		/// Adds the beliefs to this agent's workbench. Each formula is added as a unique agent perception.
		/// </summary>
		public void AddBelief(params AtomicFormula[] formula)
		{
			foreach (AtomicFormula ff in formula) 
				PerceivedEnvironementChanges.Push (new Tuple<IList, PerceptionType> (new List<AtomicFormula> (){ ff }, PerceptionType.AddBelief));
		}

		/// <summary>
		/// Adds a belief to this agent's workbench.
		/// </summary>
		/// <param name="formula">Formula.</param>
		public void AddBelief(AtomicFormula formula)
		{
			PerceivedEnvironementChanges.Push (new Tuple<IList, PerceptionType> (new List<AtomicFormula> (){ formula }, PerceptionType.AddBelief));
		}


        #endregion
    }
}