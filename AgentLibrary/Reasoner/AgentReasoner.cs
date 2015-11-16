/**
         __  __                                     _   
        |  \/  |                                   | |  
        | \  / | _   _  ___   __ _     _ __    ___ | |_ 
        | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
        | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
        |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|

*/
using System.Collections.Generic;
using Quartz;
using System.Collections;
using System.Threading;
using System;
using AgentLibrary.Networking;
using FormulaLibrary.ANTLR;
using FormulaLibrary;
using PlanLibrary;

namespace AgentLibrary
{
    /// <summary>
    /// A convinient enum to mark the changes that can occur within the environment
    /// </summary>
    public enum PerceptionType
    {
        Null                = 0x0,              //No changes occurred
        AddBelief           = 0x1,              //A belief has been added
        RemoveBelief        = 0x2,              //A belief has been removed
        UpdateBeliefValue   = 0x3,              //A belief's value must be updated
        SetBeliefValue      = 0x4,              //A belief's value must be set
        UnSetBeliefValue    = 0x5,              //A belief's value must be unset
		Achieve				= 0x6,
		Test				= 0x7
    }

    public sealed class AgentReasoner
    {
        private int currentReasoningCycle = 0;
		private readonly int ReasoningUpdateTime = 1500;

        /// <summary>
        /// The agent this reasoner belongs to
        /// </summary>
        private readonly Agent parentAgent;

		/// <summary>
		/// The events catalogue this agent reacts to. The key is a couple Formula-Perception type, while the value is a
		/// PlanModel that indicates the plan to execute.
		/// </summary>
		public Dictionary<Tuple<string, PerceptionType>, Type> EventsCatalogue
		{
			get { return eventsCatalogue; }
			private set 
			{
				lock (events_catalogue_lock) 
				{
					eventsCatalogue = value; 
				}
			}
		}
		private Dictionary<Tuple<string, PerceptionType>, Type> eventsCatalogue;
		private object events_catalogue_lock;

		/// <summary>
		/// The arguments to be passed to plans triggered by events.
		/// </summary>
		public Dictionary<Tuple<string, PerceptionType>, Dictionary<string,object>> EventsArgs
		{
			get { return events_args; }
			private set 
			{
				lock (events_args_lock) 
				{
					events_args = value; 
				}
			}
		}
		private Dictionary<Tuple<string, PerceptionType>, Dictionary<string,object>> events_args;
		private object events_args_lock;

		/// <summary>
		/// The stack containing the events to be triggered during the reasoning activity
		/// </summary>
		public Stack<Tuple<string, PerceptionType, Type>> Events 
		{
			get { return events; }
			private set 
			{
				lock (events_lock) 
				{
					events = value; 
				}
			}
		}
		private Stack<Tuple<string, PerceptionType, Type>> events;
		private object events_lock;

		/// <summary>
		/// Gets a value indicating whether this reasoner is running.
		/// </summary>
		public bool IsRunning
		{
			get { return is_running; }
			private set 
			{ 
				lock(is_running_lock)
				{
					is_running = value;	
				}
			}
		}
		private bool is_running;
		private object is_running_lock;

		/// <summary>
		/// Gets a value indicating whether this agent is paused.
		/// </summary>
		public bool Paused
		{
			get { return _paused; }
			private set 
			{ 
				_paused = value; 
				if (pause_requested)
					pause_requested = false;
			}
		}
		private bool _paused;

		/// <summary>
		/// A value indicating wheter this reasoner must be paused after the current reasoning cycle.
		/// </summary>
		private bool pause_requested;

		/// <summary>
		/// The reasoning timer. This timer is used to trigger every X milliseconds the main reasoning method
		/// </summary>
		private System.Threading.Timer reasoning_timer;

		/// <summary>
		/// Initializes a new instance of the <see cref="AgentLibrary.AgentReasoner"/> class.
		/// </summary>
		/// <param name="agent">The <see cref="AgentLibrary.Agent"/> this reasoner is related.</param>
        public AgentReasoner(Agent agent)
        {
			events_lock 	= new object ();
			events_catalogue_lock = new object ();
			events_args_lock = new object ();
			is_running_lock = new object ();

			EventsCatalogue = new Dictionary<Tuple<string, PerceptionType>, Type> ();
			Events = new Stack<Tuple<string, PerceptionType, Type>> ();
			EventsArgs = new Dictionary<Tuple<string, PerceptionType>, Dictionary<string, object>> ();

			Paused = false;
			pause_requested = false;

            parentAgent = agent;

			TimerCallback reasoning_method_callback = new TimerCallback (agentReasoningMain);
			reasoning_timer = new System.Threading.Timer (reasoning_method_callback, new object(), ReasoningUpdateTime, ReasoningUpdateTime);

        }

        internal void startReasoning()
        {
			//TODO log this
            //TODO segna un timestamp in cui inizia il reasoning
            Console.WriteLine("Starting agent [" + parentAgent.Name + "] reasoning...");
			IsRunning = true;
        }

        public void stopReasoning()
        {
			IsRunning = false;
			//TODO log this
            //TODO segna un timestamp in cui ferma il reasoning

        }

		/// <summary>
		/// Requests this reasoner to pause. More precisely, this reasoner is paused after the current reasoning cycle.
		/// </summary>
		public void Pause()
		{
			//TODO log pause reasoning
			Console.WriteLine ("### " + parentAgent.Name + " PAUSED ###");

			pause_requested = true;
		}

		/// <summary>
		/// Resume the reasoning activity.
		/// </summary>
        public void Resume()
        {
			Console.WriteLine ("### " + parentAgent.Name + " RESUMED ###");

			//Set the Paused value to false
			Paused = false;

			//Change the reasoner start time/interval
			reasoning_timer.Change (ReasoningUpdateTime, ReasoningUpdateTime);

			//TODO log resume reasoning
            //TODO segna un timestamp in cui ripristina il reasoning
            
        }

		/// <summary>
		/// This is the main agent's reasoning method. It is executed on a unique thread.
		/// </summary>
		// TODO da cambiare in agentReasoningCycle()
		private void agentReasoningMain(object state)
        {
			//wait until this reasoning cycle has finished
			reasoning_timer.Change (Timeout.Infinite, Timeout.Infinite);

			//TODO log here
            Console.WriteLine("reasoning cycle #"+ currentReasoningCycle++);
            Console.WriteLine("Checking mail box...");
            
			//Check the agent's mail box and update its workbench according to the last received message
            checkMailBox();

            Console.WriteLine("Checking environment changes...");

			Thread.Sleep (500);

			//update the agent's workbench according to the internal beliefs update



            //Update the agent workbench
            perceive();

			Thread.Sleep (500);

			triggerEvent ();

			//check events
			//triggerEvent ();
			//triggerEvent ();

            //Clear the agent's queue of perceived environment changes
			parentAgent.PerceivedEnvironementChanges.Clear();

			//check intentions (plans)

            Console.WriteLine("######### done reasoning...");
            Thread.Sleep(ReasoningUpdateTime);

			//If a pause request is pending
			if (pause_requested)
			{
				reasoning_timer.Change (Timeout.Infinite, Timeout.Infinite);
				Paused = true;
				return;
			}

			//Restore the time interval for the next reasoning cycle
			if (!Paused && !parentAgent.Busy)
				reasoning_timer.Change (ReasoningUpdateTime, ReasoningUpdateTime);
        }

        /// <summary>
        /// Checks the agent's mail box.
        /// </summary>
        private void checkMailBox()
        {
			if (parentAgent.MailBox.Count <= 0)
				return;
			
			//Take the last message within the mail box
			Tuple<AgentPassport, AgentMessage> last_message = parentAgent.MailBox.Pop ();
			AgentPassport passport 	= last_message.Item1;
			AgentMessage msg 		= last_message.Item2;

			//TODO log checked mailbox


			//TODO [importante] bisogna capire qui se un evento è interno o esterno



			//process the message
			switch (msg.InfoType)
			{
			case InformationType.Tell:
				Console.WriteLine ("[" + parentAgent.Name + "] perceiving TELL: " + msg.ToString ());
				parentAgent.Workbench.AddStatement (FormulaParser.Parse (msg.Message as string));
				break;

			case InformationType.Untell:
				Console.WriteLine ("[" + parentAgent.Name + "] perceiving UNTELL: " + msg.ToString ());
				parentAgent.Workbench.RemoveStatement (FormulaParser.Parse (msg.Message as string));
				break;
			
			case InformationType.Achieve:
                    //!!!

				break;
			}
        }

        /// <summary>
        /// Perceive the environment changes and updates the workbench of the parent agent
        /// </summary>
        private void perceive()
        {
			if (parentAgent.PerceivedEnvironementChanges.Count <= 0)
                return;

			Tuple<IList, PerceptionType> p;

			p = parentAgent.PerceivedEnvironementChanges.Pop ();
			IList changes_list = p.Item1;
			PerceptionType perception_type = p.Item2;

			switch (perception_type) 
			{
			case PerceptionType.Achieve:
				Type plan_to_execute = changes_list [0] as Type;

				Dictionary<string,object> args = null;
				EventsArgs.TryGetValue (new Tuple<string, PerceptionType> (plan_to_execute.Name, perception_type), out args);

				parentAgent.ExecutePlan (plan_to_execute, args);
				break;

			case PerceptionType.AddBelief:
				parentAgent.Workbench.AddStatement (changes_list);
				checkExternalEvents(changes_list, perception_type);
				break;

			case PerceptionType.RemoveBelief:
				parentAgent.Workbench.RemoveStatement (changes_list);
				checkExternalEvents(changes_list, perception_type);
				break;

			case PerceptionType.SetBeliefValue:
				break;

			case PerceptionType.UnSetBeliefValue:
				break;

			case PerceptionType.UpdateBeliefValue:
				break;

			case PerceptionType.Null:
				break;
			}
        }

		/// <summary>
		/// Given the perceived evironment changes, checks for the events to trigger. Only ONE event is trigged for
		/// reasoning cycle is handled.
		/// 
		/// The perceived changes in the environment could trigger (external) events.
		/// </summary>
		private void checkExternalEvents(IList changes_list, PerceptionType perception_type)
		{
			Type plan_to_execute = null;

			//Iterate each agent's perceived change in the environment
			foreach (AtomicFormula formula in changes_list) 
			{
				//Check if an handler for the perceived change exists.
				EventsCatalogue.TryGetValue (new Tuple<string, PerceptionType> (formula.ToString (), perception_type), out plan_to_execute);

				//If exists, an external event is triggered, and a specific plan is triggered.
				if (plan_to_execute != null) 
				{
					Events.Push (new Tuple<string, PerceptionType, Type> (formula.ToString (), perception_type, plan_to_execute));

					Console.WriteLine ("PUSHED EVENT " + formula.ToString () + "<->" + perception_type);
				}

				//set plan_to_execute to null for the next iteration
				plan_to_execute = null;
			}
		}

        /// <summary>
		/// Trigger the top event within the events stack. Events may be triggered by the agent's perception of changes
		/// within the environment it's located. 
        /// </summary>
		private void triggerEvent()
        {
			//Do nothing if the events stack is empty
			if (Events.Count <= 0)
				return;

			//Get the top event, and its info
			Tuple<string, PerceptionType, Type> eventTuple = Events.Pop ();

			string formula 					= eventTuple.Item1;
			PerceptionType perception_type 	= eventTuple.Item2;
			Type plan_to_execute 			= eventTuple.Item3;

			//Try get values related to this event
			Dictionary<string,object> args = null;
			EventsArgs.TryGetValue (new Tuple<string, PerceptionType> (formula, perception_type), out args);

			//Execute the plan (if exists)
			parentAgent.ExecutePlan (plan_to_execute, args);
        }

		/// <summary>
		/// Adds an event.
		/// </summary>
		/// <param name="formula">The formula to reason on.</param>
		/// <param name="perception">The perception this event reacts to.</param>
		/// <param name="Plan">The plan to execute.</param>
		public void AddEvent(string formula, PerceptionType perception, Type Plan, Dictionary<string,object> Args = null)
		{
			//If Plan is not of type PlanModel, then throw an exception
			if (!Plan.BaseType.IsEquivalentTo (typeof(PlanModel)))
				throw new Exception ("Argument #3 in AddEvent(...) must be of type PlanModel.");
			
			Tuple<string,PerceptionType> the_key = new Tuple<string, PerceptionType> (formula, perception);

			//Check if the event is already contained within the agent's event catalogue
			if (EventsCatalogue.ContainsKey (the_key))
				return;

			//Add the event
			EventsCatalogue.Add (the_key, Plan);

			//Add the event's args
			EventsArgs.Add (the_key, Args);
		}
    }
}