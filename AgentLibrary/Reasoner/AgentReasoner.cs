//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  AgentReasoner.cs
//
//  Author:
//       Davide Guastella <davide.guastella90@gmail.com>
//
//  Copyright (c) 2015 Davide Guastella
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.Collections;
using System.Threading;
using System;
using AgentLibrary;
using MusaCommon;
using System.Text;

namespace AgentLibrary
{
    public sealed class AgentReasoner
    {
        #region Fields/Properties

        //TODO questi campi andranno rimossi in futuro
        private int currentReasoningCycle = 0;
        private readonly int ReasoningUpdateTime = 2000;

        /// <summary>
        /// The agent this reasoner belongs to
        /// </summary>
        private readonly Agent parentAgent;

        /// <summary>
        /// The collection of events this agent reacts to. The key of this 
        /// collection is an event key, made of a formula-perception tuple that
        /// is needed to trigger an event. The value (Type) is the class type
        /// of the plan to be invoked when an event is triggered.
        /// This collection represents the knowledge of the agent to react on
        /// specific perception.
        /// </summary>
        public Dictionary<AgentEventKey, Type> EventsCatalogue
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

        private Dictionary<AgentEventKey, Type> eventsCatalogue;
        private object events_catalogue_lock;

        /// <summary>
        /// The collection of arguments that have to be passed to events when 
        /// these are triggered. It associates an event key (a formula-perception pair)
        /// with a collection of arguments that are passed to the plan invoked by
        /// the event.
        /// </summary>
        public Dictionary<AgentEventKey, PlanArgs> EventsArgs
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

        private Dictionary<AgentEventKey, PlanArgs> events_args;
        private object events_args_lock;

        /// <summary>
        /// This collection contains the events that are perceived during the 
        /// reasoning activity of the agent. As the agent has knowledge of the 
        /// events to which it should react to, when a certain perception (of 
        /// which he is aware) occurs, the corresponding event is added to this 
        /// collection, and therefore it's triggered during the reasoning cycle 
        /// of the agent.
        /// </summary>
        private Stack<Tuple<string, AgentPerception, Type>> PerceivedEvents
        {
            get { return perceived_events; }
            set
            {
                lock (events_lock)
                {
                    perceived_events = value; 
                }
            }
        }

        private Stack<Tuple<string, AgentPerception, Type>> perceived_events;
        private object events_lock;

        /// <summary>
        /// Gets a value indicating whether this reasoner is running.
        /// </summary>
        public bool IsRunning
        {
            get { return is_running; }
            private set
            { 
                lock (is_running_lock)
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
        /// The logger set this reasoner uses to log its activities.
        /// </summary>
        private readonly ILogger Logger;

        #endregion Fields/Properties

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentLibrary.AgentReasoner"/> class.
        /// </summary>
        /// <param name="agent">The <see cref="AgentLibrary.Agent"/> this reasoner is related.</param>
        public AgentReasoner(Agent agent)
        {
            events_lock = new object();
            events_catalogue_lock = new object();
            events_args_lock = new object();
            is_running_lock = new object();

            EventsCatalogue = new Dictionary<AgentEventKey, Type>();
            PerceivedEvents = new Stack<Tuple<string, AgentPerception, Type>>();
            EventsArgs = new Dictionary<AgentEventKey, PlanArgs>();

            Paused = false;
            pause_requested = false;

            parentAgent = agent;

            TimerCallback reasoning_method_callback = new TimerCallback(reasoningCycleMain);
            reasoning_timer = new Timer(reasoning_method_callback, new object(), ReasoningUpdateTime, ReasoningUpdateTime);

            //Inject the logger
            Logger = ModuleProvider.Get().Resolve<ILogger>();
        }

        #endregion Constructor

        #region Methods

        internal void startReasoning()
        {
            Logger.Log(LogLevel.Trace, "[" + parentAgent.Name + "] starts reasoning");
            IsRunning = true;
        }

        public void stopReasoning()
        {
            Logger.Log(LogLevel.Trace, "[" + parentAgent.Name + "] stops reasoning");
            IsRunning = false;

        }

        /// <summary>
        /// Requests this reasoner to pause. More precisely, this reasoner is paused after the current reasoning cycle.
        /// </summary>
        public void Pause()
        {
            Logger.Log(LogLevel.Trace, "[" + parentAgent.Name + "] paused");
            pause_requested = true;
        }

        /// <summary>
        /// Resume the reasoning activity.
        /// </summary>
        public void Resume()
        {
            Logger.Log(LogLevel.Trace, "[" + parentAgent.Name + "] resumed");

            //Set the Paused value to false
            Paused = false;

            //Change the reasoner start time/interval
            reasoning_timer.Change(ReasoningUpdateTime, ReasoningUpdateTime);
        }

        /// <summary>
        /// This is the main agent's reasoning method. It is executed on a unique thread.
        /// </summary>
        private void reasoningCycleMain(object state)
        {
            //wait until this reasoning cycle has finished
            reasoning_timer.Change(Timeout.Infinite, Timeout.Infinite);

            Logger.SetColorForNextConsoleLog(ConsoleColor.Black, ConsoleColor.DarkBlue);

            Logger.Log(LogLevel.Trace, "[" + parentAgent.Name + "] reasoning cycle #" + currentReasoningCycle++);
            Logger.Log(LogLevel.Trace, "[" + parentAgent.Name + "] Checking mailbox");
            //Check the agent's mail box and update its workbench according to the last received message
            checkMailBox();

            //Perceive the environment changes
            Logger.Log(LogLevel.Trace, "[" + parentAgent.Name + "] Perceiving environment changes");
            perceive();

            //Trigger potential events produced by this agent's environment perception
            Logger.Log(LogLevel.Trace, "[" + parentAgent.Name + "] Triggering events");
            triggerEvent();

            Logger.SetColorForNextConsoleLog(ConsoleColor.Black, ConsoleColor.DarkBlue);
            Logger.Log(LogLevel.Trace, "[" + parentAgent.Name + "] done reasoning");
            Logger.SetColorForNextConsoleLog(ConsoleColor.Black, ConsoleColor.DarkBlue);
            Logger.Log(LogLevel.Trace, "[" + parentAgent.Name + "] ##############");

            Thread.Sleep(ReasoningUpdateTime);

            //If a pause request is pending
            if (pause_requested)
            {
                reasoning_timer.Change(Timeout.Infinite, Timeout.Infinite);
                Paused = true;
                return;
            }

            //Restore the time interval for the next reasoning cycle
            if (!Paused && !parentAgent.Busy)
                reasoning_timer.Change(ReasoningUpdateTime, ReasoningUpdateTime);
        }

        /// <summary>
        /// Checks the agent's mail box.
        /// </summary>
        private void checkMailBox()
        {
            if (parentAgent.MailBox.Count <= 0)
                return;
			
            //Take the last message within the mail box
            Tuple<AgentPassport, AgentMessage> last_message = parentAgent.MailBox.Pop();
            AgentPassport sender_agent_passport = last_message.Item1;
            AgentMessage msg = last_message.Item2;
            //TODO [importante] bisogna capire qui se un evento è interno o esterno (vedi documentazione jason/agentspeak)

            Logger.SetColorForNextConsoleLog(ConsoleColor.Black, ConsoleColor.Green);

            var FormulaParser = ModuleProvider.Get().Resolve<IFormulaUtils>();

            //process the message
            switch (msg.InfoType)
            {
                case InformationType.Tell:
                    Logger.Log(LogLevel.Debug, "[" + parentAgent.Name + "] perceiving TELL: " + msg);
                    parentAgent.AddBelief(FormulaParser.Parse(msg.Message as string));
                    break;

                case InformationType.Untell:
                    Logger.Log(LogLevel.Debug, "[" + parentAgent.Name + "] perceiving UNTELL: " + msg);
                    parentAgent.RemoveBelief(FormulaParser.Parse(msg.Message as string));
                    break;
			
                case InformationType.Achieve:
                    if (string.IsNullOrEmpty(msg.Message as string))
                        throw new Exception("Received an empty message. Cannot achieve any goal.");

                    Type planToExecute = parentAgent.Plans.Find(x => x.Name.Equals(msg.Message as string));
                    //TODO i parametri do stanno?
                    PlanArgs args = null;

                    //Achieve the goal
                    achieveGoal(planToExecute, args);
                    break;

                //case InformationType.AskOne:
                //    //msg contiene una formula
                //    //formula --> IFormula
                //    //testa formula in workbench
                //    //manda risultato a sender_agent_passport
                //    break;

                //case InformationType.AskAll:
                //    break;

            }
        }

        /// <summary>
        /// Achieves a goal.
        /// </summary>
        /// <param name="planToExecute">Plan to execute.</param>
        /// <param name="args">Arguments.</param>
        private void achieveGoal(Type planToExecute, PlanArgs args = null)
        {
            //Check if user has been set any default argument to be passed when the agent perceive 
            //an Achieve type perception togheter with the specified plan. If any argument is found, is added
            //to the input argument list.
            PlanArgs defaultArgs;
            EventsArgs.TryGetValue(new AgentEventKey(planToExecute.Name, AgentPerception.Achieve), out defaultArgs);

            //Merge the input args and the default args
            if (args != null)
                defaultArgs.Merge(args);

            Logger.SetColorForNextConsoleLog(ConsoleColor.Black, ConsoleColor.Yellow);
            Logger.Log(LogLevel.Debug, "[" + parentAgent.Name + "] Achieving goal " + planToExecute.Name);

            //Execute the input plan to achieve the goal
            parentAgent.ExecutePlan(planToExecute, defaultArgs);
        }

        /// <summary>
        /// Perceive the environment changes and updates the workbench of the 
        /// parent agent
        /// </summary>
        private void perceive()
        {
            if (parentAgent.PerceivedEnvironementChanges.Count <= 0)
                return;

            //Get the last change within the environment
            Tuple<IList, AgentPerception> p = parentAgent.PerceivedEnvironementChanges.Pop();
            IList changes_list = p.Item1;
            AgentPerception perception_type = p.Item2;

            switch (perception_type)
            {
                case AgentPerception.Achieve:
                    //Get the agent which has to be executed
                    Type plan_to_execute = changes_list[0] as Type;

                    //Achieve the goal by executing the plan
                    achieveGoal(plan_to_execute);
                    break;

                case AgentPerception.AddBelief:
                    //Tell the agent to add the beliefs to its workbench
                    parentAgent.Workbench.AddStatement(changes_list);
                    checkExternalEvents(changes_list, perception_type);
                    break;

                case AgentPerception.RemoveBelief:
                    parentAgent.Workbench.RemoveStatement(changes_list);
                    checkExternalEvents(changes_list, perception_type);
                    break;

                case AgentPerception.UpdateBelief:
                    parentAgent.Workbench.UpdateStatement(changes_list);
                    break;

                case AgentPerception.Null:
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Given the perceived evironment changes, checks for the events to trigger. Only ONE event is trigged for
        /// reasoning cycle is handled.
        /// 
        /// The perceived changes in the environment could trigger (external) events.
        /// </summary>
        private void checkExternalEvents(IList changes_list, AgentPerception perception_type)
        {
            Type plan_to_execute = null;

            //Iterate each agent's perceived change in the environment
            foreach (IAtomicFormula formula in changes_list)
            {
                //Check if an handler for the perceived change exists.
                EventsCatalogue.TryGetValue(new AgentEventKey(formula.ToString(), perception_type), out plan_to_execute);

                //If exists, an external event is triggered, and a specific plan is triggered.
                if (plan_to_execute != null)
                    PerceivedEvents.Push(new Tuple<string, AgentPerception, Type>(formula.ToString(), perception_type, plan_to_execute));

                //set plan_to_execute to null for the next iteration
                plan_to_execute = null;
            }
        }

        /// <summary>
        /// Trigger the event on top of the perceived event stack.
        /// </summary>
        private void triggerEvent()
        {
            //Do nothing if the events stack is empty
            if (PerceivedEvents.Count <= 0)
                return;

            //Get the top event, and its info
            Tuple<string, AgentPerception, Type> eventTuple = PerceivedEvents.Pop();

            string formula = eventTuple.Item1;
            AgentPerception perception_type = eventTuple.Item2;
            Type plan_to_execute = eventTuple.Item3;

            Logger.SetColorForNextConsoleLog(ConsoleColor.Black, ConsoleColor.Cyan);
            Logger.Log(LogLevel.Debug, "[{0}] Triggering event {" + "[" + parentAgent.Name + "] Triggering event {" + formula + "->" + perception_type + "->" + plan_to_execute.Name + "}");

            //Try get values related to this event
            PlanArgs args = null;
            EventsArgs.TryGetValue(new AgentEventKey(formula, perception_type), out args);

            //Execute the plan (if exists)
            parentAgent.ExecutePlan(plan_to_execute, args);
        }

        /// <summary>
        /// Adds an event. The event added becomes part of the agent's 
        /// knowledge.
        /// </summary>
        /// <param name="ev">The event to be added.</param>
        public void AddEvent(AgentEvent ev)
        {
            AddEvent(ev.Formula, ev.Perception, ev.Plan, ev.Args);
        }

        /// <summary>
        /// Adds an event. The event added becomes part of the agent's 
        /// knowledge.
        /// </summary>
        /// <param name="formula">The formula.</param>
        /// <param name="perception">The perception this event reacts to.</param>
        /// <param name="Plan">The plan to execute.</param>
        /// <param name="Args">The arguments to be passed to the plan invoked
        /// when this event is being triggered.</param>
        public void AddEvent(string formula, AgentPerception perception, Type Plan, PlanArgs Args = null)
        {
            //If Plan is not of type PlanModel, then throw an exception
            if (!(typeof(IPlanModel).IsAssignableFrom(Plan)))
                throw new Exception("Argument #3 in AddEvent(...) must inherits from PlanModel.");
			
            AgentEventKey the_key = new AgentEventKey(formula, perception);

            //Check if the event is already contained within the agent's event catalogue
            if (EventsCatalogue.ContainsKey(the_key))
                return;

            Logger.SetColorForNextConsoleLog(ConsoleColor.Black, ConsoleColor.DarkCyan);
            Logger.Log(LogLevel.Debug, "[" + parentAgent.Name + "] Added event {key}" + the_key + " {plan}" + Plan.Name);

            //Add the event
            EventsCatalogue.Add(the_key, Plan);

            //Add the event's args
            EventsArgs.Add(the_key, Args);
        }

        #endregion Methods
    }
}