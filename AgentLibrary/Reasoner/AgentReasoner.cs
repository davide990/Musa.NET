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
        UnSetBeliefValue    = 0x5               //A belief's value must be unset
    }

    public sealed class AgentReasoner
    {
        private int currentReasoningCycle = 0;
		private readonly int ReasoningUpdateTime = 5000;

        /// <summary>
        /// The agent this reasoner belongs to
        /// </summary>
        private readonly Agent parentAgent;

        /// <summary>
        /// Gets or sets the agent thread.
        /// </summary>
        private Thread AgentThread
        {
			get { return parentAgent.Thread; }
			set { parentAgent.Thread = value; }
        }

		/// <summary>
		/// Gets the the set of intentions of the agent, that is, the set of plans that the agent is intended to 
		/// execute.
		/// </summary>
		public Queue<Type> AgentIntentions
		{
			get { return intentions; }
			set 
			{ 
				lock(intentions_lock)
				{
					intentions = value;	
				}
			}
		}
		private Queue<Type> intentions;
		private object intentions_lock;

		/// <summary>
		/// The events this agent reacts to. The key is a couple Formula-Perception type, while the value is a PlanModel
		///  that indicates the plan to execute.
		/// </summary>
		public Dictionary<Tuple<string,PerceptionType>, Type> Events
		{
			get { return events; }
			private set 
			{
				lock(events_lock)
				{
					events = value; 
				}
			}
		}
		private Dictionary<Tuple<string,PerceptionType>, Type> events;
		private object events_lock;


        public AgentReasoner(Agent agent)
        {
			intentions_lock = new object ();
			events_lock 	= new object ();

			Events = new Dictionary<Tuple<string, PerceptionType>, Type> ();

			AgentIntentions = new Queue<Type> ();

            parentAgent 	= agent;
            AgentThread      = new Thread(new ThreadStart(agentReasoningMain));
            AgentThread.Name = parentAgent.Name;
        }

        internal void startReasoning()
        {
			//TODO log this
            //TODO segna un timestamp in cui inizia il reasoning
            Console.WriteLine("Starting agent [" + parentAgent.Name + "] reasoning...");
            AgentThread.Start();
        }

        public void stopReasoning()
        {
			//TODO log this
            //TODO segna un timestamp in cui ferma il reasoning

        }

        public void pauseReasoning()
        {
			//TODO log this
            //TODO segna un timestamp in cui pausa il reasoning
            
        }

        public void resumeReasoning()
        {
			//TODO log this
            //TODO segna un timestamp in cui ripristina il reasoning
            
        }

		/// <summary>
		/// This is the main agent's reasoning method. It is executed on a unique thread.
		/// </summary>
        private void agentReasoningMain()
        {
            while(true)
            {
				//TODO log here
                Console.WriteLine("reasoning cycle #"+ currentReasoningCycle++);
                Console.WriteLine("Checking mail box...");
                
				//Check the agent's mail box and update its workbench according to the last received message
                checkMailBox();

                Console.WriteLine("Checking environment changes...");

                //Update the agent workbench
                updateWorkbenchChanges();

				//check events
				checkEvents ();

                //Trigger possible events caused by the update of agent's workbench
                triggerEvents();

                //Clear the agent's queue of perceived environment changes
				parentAgent.PerceivedEnvironementChanges.Clear();



				//check intentions (plans)


                Console.WriteLine("######### done reasoning...");
                Thread.Sleep(ReasoningUpdateTime);
                //#Environment percept
                //-> percept belief changes in environment
                //-> percept/trigger events
                //#Jobs scheduling
                //-> dequeue and execute
            }
        }

		/// <summary>
		/// Checks the events.
		/// </summary>
		private void checkEvents()
		{
			/**
				per ogni messaggio MSG ricevuto
				{
					Se MSG è contenuto in events e il tipo di messaggio è uguale al tipo di percezione dell'evento,
					  aggiungi il piano corrispondente alle intenzioni
				}

			*/

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
			AgentPassport passport = last_message.Item1;
			AgentMessage msg = last_message.Item2;

			//TODO log checked mailbox

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
        /// Percept the environment changes and updates the workbench of the parent agent
        /// </summary>
        private void updateWorkbenchChanges()
        {
			if (parentAgent.PerceivedEnvironementChanges.Count <= 0)
                return;

			foreach (Tuple<IList, PerceptionType> p in parentAgent.PerceivedEnvironementChanges) 
			{
				IList changes_list = p.Item1;
				PerceptionType perception_type = p.Item2;

				switch (perception_type) 
				{
				case PerceptionType.AddBelief:
					//Add the statement to the agent's workbench
					parentAgent.Workbench.AddStatement (changes_list);




					//Check for event 
					/*foreach (AtomicFormula formula in p.Key)
					{
						Console.WriteLine ("[" + parentAgent.Name + "] perceiving " + p.Value.ToString () + ": " + formula.ToString());

						Type plan_to_execute = null;
						Events.TryGetValue (new Tuple<string, PerceptionType> (formula.ToString (), p.Value), out plan_to_execute);
						if (plan_to_execute != null) 
							AgentIntentions.Enqueue (plan_to_execute);
					}*/
					break;

				case PerceptionType.RemoveBelief:
					parentAgent.Workbench.AddStatement (changes_list);
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
        }

        /// <summary>
        /// Trigger events. Events triggers may be caused by the agent's perception of changes within the environment.
        /// </summary>
        private void triggerEvents()
        {
			if (parentAgent.PerceivedEnvironementChanges.Count <= 0)
                return;

			foreach (Tuple<IList, PerceptionType> p in parentAgent.PerceivedEnvironementChanges) 
            {
				IList changes_list = p.Item1;
				PerceptionType perception_type = p.Item2;

                switch (perception_type)
                {
                    case PerceptionType.AddBelief:
                        //TRIGGER
                        break;

                    case PerceptionType.RemoveBelief:
                        //TRIGGER
                        break;

                    case PerceptionType.SetBeliefValue:
                        //TRIGGER
                        break;

                    case PerceptionType.UnSetBeliefValue:
                        //TRIGGER
                        break;

                    case PerceptionType.UpdateBeliefValue:
                        //TRIGGER
                        break;

                    case PerceptionType.Null:
                        break;
                }
            }
        }


		/// <summary>
		/// Adds an event.
		/// </summary>
		/// <param name="formula">The formula to reason on.</param>
		/// <param name="perception">The perception this event reacts to.</param>
		/// <param name="Plan">The plan to execute.</param>
		public void AddEvent(string formula, PerceptionType perception, Type Plan)
		{
			//If Plan is not of type PlanModel, then throw an exception
			if (!Plan.BaseType.IsEquivalentTo (typeof(PlanModel)))
				throw new Exception ("Argument #3 in AddEvent(...) must be of type PlanModel.");

			//Add the event
			Events.Add (new Tuple<string, PerceptionType> (formula, perception), Plan);
		}
    }
}