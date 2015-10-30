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
        /// The list of jobs this agent is appointed to do
        /// </summary>
        private List<AgentJob> jobs;

        /// <summary>
        /// A unique identifier for this agent
        /// </summary>
        private readonly Guid ID;

        /// <summary>
        /// The date in which this agent started its life cycle.
        /// </summary>
		public DateTime CreatedAt
		{
			get { return createdAt; }
		}
		private readonly DateTime createdAt;

		/// <summary>
		/// The agent thread.
		/// </summary>
		internal Thread Thread
		{
			get { return agent_thread; }
			set { agent_thread = value; }
		}
        private Thread agent_thread;

        /// <summary>
        /// This queue contains the changes to the environment that this agent have to perceive. Since an agent can be busy in doing other
        /// activities such event-handling or execution of plans, the perceived environment changes are accumulated temporarily in this
        /// queue until the agent start a perception activity.
        /// </summary>
        internal Dictionary<IList, PerceptionType> perceivedEnvironementChanges;
        internal object lock_perceivedEnvironmentChanges = new object();

        /// <summary>
        /// This dictionary represents the mailbox of the agent. In this dictionary are collected all the messages that the agent may receive
        /// from other agents in the same, or different, environment. The messages are read during the agent's reasoning cycle.
        /// </summary>
        internal Dictionary<AgentPassport, AgentMessage> mailBox;
        internal object lock_mailBox = new object();

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
		/// The set of plans that this agent is executing.
		/// </summary>
		private HashSet<string> ExecutingPlans;

		/// <summary>
		/// Gets a value indicating whether this agent is busy (executing a plan).
		/// </summary>
		public bool Busy
		{
			get { return agent_is_busy; }
			private set {agent_is_busy = value; }
		}
		private bool agent_is_busy;

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
		/// Check if actually is working time for this agent.
		/// </summary>
		public bool IsActive
		{
			get { DateTime now = DateTime.UtcNow; return now >= WorkScheduleStart && now < WorkScheduleEnd; }
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
		/// Return all the messages received by this agent
		/// </summary>
		public List<AgentMessage> MailBox
		{
			get { return new List<AgentMessage>(mailBox.Values); }
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

		#endregion

        #region Events

        /// <summary>
        /// Event triggered when this agent receive a new job
        /// </summary>
        public event EventHandler jobReceived;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new agent
        /// </summary>
        /// <param name="agent_name"></param>
        public Agent(string agent_name)
        {
            name = agent_name;
            ID = Guid.NewGuid();
            jobReceived += onJobReceived;

            jobs = new List<AgentJob>();
            roles = new List<AgentRole>();
            workbench = new AgentWorkbench(this);
            reasoner = new AgentReasoner(this);
            perceivedEnvironementChanges = new Dictionary<IList, PerceptionType>();
            mailBox = new Dictionary<AgentPassport, AgentMessage>();
			createdAt = DateTime.Now;

			PlansCollection = new Dictionary<Type, IPlanInstance> ();

			Busy = false;
        }

        #endregion

        #region Destructors

        ~Agent()
        {
            //TODO notify agent retirement
            jobs.Clear();
            roles.Clear();

            //scheduler.Shutdown();
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
        /// <param name="changes">The data that involved in the environment change</param>
        internal void notifyEnvironementChanges(PerceptionType action, IList changes)
        {
			//TODO log environment change
            Console.WriteLine("[Agent " + name + "] received " + action.ToString() + " -> " + changes.ToString());
            perceivedEnvironementChanges.Add(changes, action);
        }

        /// <summary>
        /// Method triggered when this agent schedule a new job.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onJobReceived(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Start the agent activity
        /// </summary>
        public Agent Start()
        {
            //begin the agent reasoning activity
            reasoner.startReasoning();

            return this;
        }

		/// <summary>
		/// Adds a plan to this agent.
		/// </summary>
		/// <returns><c>true</c>, if plan was added, <c>false</c> otherwise.</returns>
		/// <param name="plan">The plan model to be added. An instance of this plan is instantiated</param>
		public void AddPlan(Type Plan)
		{
			if (!Plan.BaseType.IsEquivalentTo (typeof(PlanModel)))
				throw new Exception ("Argument #1 in ExecutePlan(...) is not of type PlanModel.");
			
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

		/// <summary>
		/// Invoked when a plan terminates its execution.
		/// </summary>
		/// <param name="sender">The executed plan.</param>
		/// <param name="args">Arguments.</param>
		private void onPlanInstanceFinished(object sender, EventArgs args)
		{
			//TODO log [agent + plan + event->finished]
			Console.WriteLine ("[Agent " + Name + "] plan " + (sender as IPlanInstance).GetName() + " finished its execution.");

			//Set the agent busy state to false
			Busy = false;
		}

		/// <summary>
		/// Invoked when a plan requests to register a result.
		/// </summary>
		/// <param name="result">Result.</param>
		private void onPlanInstanceRegisterResult(string result)
		{
			Formula resultFormula = null;

			try
			{
				resultFormula = FormulaParser.Parse(result);
				Workbench.AddStatement(resultFormula);
			}
			catch(Exception e)
			{
				Console.WriteLine ("Unable to parse formula '" + result + "'.\n" + e.Message);
			}

			//TODO convert result to formula, then add it to agent's workbench
		}

		/// <summary>
		/// Executes a plan.
		/// </summary>
		/// <param name="Plan">Plan.</param>
		/// <param name="args">Arguments.</param>
		public void ExecutePlan(Type Plan, Dictionary<string, object> args = null)
		{
			//If Plan is not of type PlanModel, then throw an exception
			if (!Plan.BaseType.IsEquivalentTo (typeof(PlanModel)))
				throw new Exception ("Argument #1 in ExecutePlan(...) is not of type PlanModel.");
			
			Type planInstanceType = typeof(PlanInstance<>).MakeGenericType (Plan);
			IPlanInstance the_plan = null;

			//Search for the input plan
			PlansCollection.TryGetValue(Plan, out the_plan );

			//If the input plan has not been found within the agent's plans collection, throw an exception
			if (the_plan == null)
				throw new Exception ("Plan '" + Plan.Name + "' not found in agent [" + Name + "] plans collection.");

			//Get the plan's Execute() method
			MethodInfo execute_method = planInstanceType.GetMethod ("Execute", BindingFlags.Public | BindingFlags.Instance);

			//Set the agent to busy
			Busy = true;

			//Execute the plan
			execute_method.Invoke (the_plan, new object[]{ args });
		}

        #endregion
    }
}