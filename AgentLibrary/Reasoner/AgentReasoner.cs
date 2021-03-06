﻿//          __  __                                     _   
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
using System.Linq;

namespace AgentLibrary
{
    public sealed class AgentReasoner
    {
        #region Fields/Properties

        //TODO questi campi andranno rimossi in futuro
        private int currentReasoningCycle = 0;
        private readonly int ReasoningUpdateTime = 1000;

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
        private Stack<AgentPerception> PerceivedEvents
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

        private Stack<AgentPerception> perceived_events;
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
            PerceivedEvents = new Stack<AgentPerception>();
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

            if (currentReasoningCycle <= 1)
                parentAgent.onInit();
        }

        /// <summary>
        /// Checks the agent's mail box.
        /// </summary>
        private void checkMailBox()
        {
            if (parentAgent.MailBoxCount <= 0)
                return;

            //Take the last message within the mail box
            Tuple<AgentPassport, AgentMessage> last_message = parentAgent.GetLastMessageInMailBox();
            AgentPassport sender_agent_passport = last_message.Item1;
            AgentMessage msg = last_message.Item2;

            //Exit if an empty message has been received
            if (msg.InformationsCount == 0)
                return;

            var FormulaParser = ModuleProvider.Get().Resolve<IFormulaUtils>();

            //TODO [importante] bisogna capire qui se un evento è interno o esterno (vedi documentazione jason/agentspeak)

            Logger.SetColorForNextConsoleLog(ConsoleColor.Black, ConsoleColor.Green);
            dynamic messageContent = msg.GetInformation();

            //process the message (AskOne and AskAll are processed separately
            switch (msg.InfoType)
            {
                case InformationType.Tell:
                    Logger.Log(LogLevel.Debug, "[" + parentAgent.Name + "] perceiving TELL: " + msg);
                    if (messageContent is string)
                        messageContent = FormulaParser.Parse(messageContent as string);

                    parentAgent.AddBelief(messageContent);
                    break;

                case InformationType.Untell:
                    Logger.Log(LogLevel.Debug, "[" + parentAgent.Name + "] perceiving UNTELL: " + msg);
                    if (msg.GetInformation() is string)
                        messageContent = FormulaParser.Parse(messageContent as string);

                    parentAgent.RemoveBelief(messageContent);
                    break;

                case InformationType.Achieve:
                    HandleAchieveTypeMessage(sender_agent_passport, msg);
                    break;
            }
        }

        /// <summary>
        /// Handle the messages of type "Achieve"
        /// </summary>
        /// <param name="sender">The sender of the message</param>
        /// <param name="msg">The message</param>
        private void HandleAchieveTypeMessage(AgentPassport sender, AgentMessage msg)
        {
            dynamic messageContent = msg.GetInformation();

            if (string.IsNullOrEmpty(messageContent))
                throw new Exception("Received an empty message. Cannot achieve any goal.");

            //Get the formula inside the message
            var the_formula = ModuleProvider.Get().Resolve<IFormulaUtils>().Parse(messageContent) as IAtomicFormula;

            //The plan to execute is the functor of the received atomic formula
            var plan_name = the_formula.GetFunctor();

            //Get the valued terms into the formula
            var valuedTerms = the_formula.GetTerms().Where(x => !x.IsLiteral()).ToList();

            //Get the plan's type 
            Type planToExecute = parentAgent.Plans.Find(x => x.Name.Equals(plan_name));

            //Get the parameters name of the parameters of the plan
            var planParamNames = ModuleProvider.Get().Resolve<IPlanFacade>().GetPlanParameter(planToExecute);

            //Link plan's parameters to the passed user values
            PlanArgs args = new PlanArgs();
            int limit = Math.Min(planParamNames.Count, valuedTerms.Count);
            for (int i = 0; i < limit; i++)
                args.Add(planParamNames[i], valuedTerms[i].GetValue());

            //Add the other args contained into the message
            foreach (var a in msg.Args)
                args.Add(a.Key, a.Value);

            //Achieve the goal
            achieveGoal(planToExecute, sender, args);
        }

        /// <summary>
        /// Achieves a goal.
        /// </summary>
        /// <param name="planToExecute">Plan to execute.</param>
        /// <param name="sourceAgent">The source agent which communicated a knownledge for which the plan is being invoked. If null, 
        /// this agent is executing the plan</param>
        /// <param name="args">Arguments.</param>
        private void achieveGoal(Type planToExecute, AgentPassport sourceAgent, PlanArgs args = null)
        {
            //Check if user has been set any default argument to be passed when the agent perceive 
            //an Achieve type perception togheter with the specified plan. If any argument is found, is added
            //to the input argument list.
            PlanArgs defaultArgs;
            EventsArgs.TryGetValue(new AgentEventKey(planToExecute.Name, AgentPerceptionType.Achieve), out defaultArgs);

            if (defaultArgs == null)
                defaultArgs = new PlanArgs();
            //Merge the input args and the default args
            if (args != null)
                defaultArgs.Merge(args);

            Logger.SetColorForNextConsoleLog(ConsoleColor.Black, ConsoleColor.Yellow);
            Logger.Log(LogLevel.Debug, "[" + parentAgent.Name + "] Achieving goal " + planToExecute.Name);

            //Execute the input plan to achieve the goal
            parentAgent.ExecutePlan(planToExecute, sourceAgent, defaultArgs);
        }

        /// <summary>
        /// Perceive the environment changes and updates the workbench of the 
        /// parent agent
        /// </summary>
        private void perceive()
        {
            Tuple<IList, AgentPerceptionType> p;
            lock (parentAgent.lock_perceivedEnvironmentChanges)
            {
                if (parentAgent.PerceivedEnvironementChanges.Count <= 0)
                    return;

                //Get the last change within the environment
                //Tuple<IList, AgentPerceptionType> p = parentAgent.PerceivedEnvironementChanges.Pop();
                p = parentAgent.PerceivedEnvironementChanges.Dequeue();
            }
            
            IList changes_list = p.Item1;
            AgentPerceptionType perception_type = p.Item2;

            switch (perception_type)
            {
                case AgentPerceptionType.Achieve:
                    //Get the agent which has to be executed
                    Type plan_to_execute = changes_list[0] as Type;

                    //Achieve the goal by executing the plan
                    achieveGoal(plan_to_execute, null);
                    break;

                case AgentPerceptionType.AddBelief:
                    //Tell the agent to add the beliefs to its workbench
                    parentAgent.Workbench.AddStatement(changes_list);
                    checkExternalEvents(changes_list, perception_type);
                    break;

                case AgentPerceptionType.RemoveBelief:
                    parentAgent.Workbench.RemoveStatement(changes_list);
                    checkExternalEvents(changes_list, perception_type);
                    break;

                case AgentPerceptionType.UpdateBelief:
                    parentAgent.Workbench.UpdateStatement(changes_list);
                    break;

                case AgentPerceptionType.Null:
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
        private void checkExternalEvents(IList changes_list, AgentPerceptionType perception_type)
        {
            Type plan_to_execute = null;

            var allEventsFormula = EventsCatalogue.Keys;
            var fp = ModuleProvider.Get().Resolve<IFormulaUtils>();

            //Iterate each agent's perceived change in the environment
            foreach (IAtomicFormula formula in changes_list)
            {
                //For each registered event in the events catalogue
                foreach (var Event in EventsCatalogue)
                {
                    //Check if the formula of an event match with the perceived formula
                    List<IAssignment> assignments;
                    if (!formula.MatchWith(fp.Parse(Event.Key.Formula), out assignments))
                        continue;

                    //If a match occour, unify the 
                    var trigger = formula.Clone() as IFormula;
                    trigger.Unify(assignments);

                    //Search for the plan that has to be invoked as result of the event
                    EventsCatalogue.TryGetValue(new AgentEventKey(Event.Key.Formula, perception_type), out plan_to_execute);

                    //Add a new event to the event stack
                    if (plan_to_execute != null)
                        PerceivedEvents.Push(new AgentPerception(trigger.ToString(), perception_type, plan_to_execute, assignments));

                    //set plan_to_execute to null for the next iteration
                    plan_to_execute = null;
                }
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
            AgentPerception perception = PerceivedEvents.Pop();

            string formula = perception.Formula;
            AgentPerceptionType perception_type = perception.PerceptionType;
            Type plan_to_execute = perception.PlanToExecute;

            Logger.SetColorForNextConsoleLog(ConsoleColor.Black, ConsoleColor.Cyan);
            Logger.Log(LogLevel.Debug, "[{0}] Triggering event {" + "[" + parentAgent.Name + "] Triggering event {" + formula + "->" + perception_type + "->" + plan_to_execute.Name + "}");

            //Try get values related to this event
            PlanArgs args;
            EventsArgs.TryGetValue(new AgentEventKey(formula, perception_type), out args);

            if (args == null)
                args = new PlanArgs();

            args.Add(perception.Args.ToArray());

            //Execute the plan (if exists)
            parentAgent.ExecutePlan(plan_to_execute, null, args);
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
        public void AddEvent(string formula, AgentPerceptionType perception, Type Plan, PlanArgs Args = null)
        {
            //If Plan is not of type PlanModel, then throw an exception
            if (!(typeof(IPlanModel).IsAssignableFrom(Plan)))
                throw new Exception("Argument #3 in AddEvent(...) must inherits from PlanModel.");

            AgentEventKey the_key = new AgentEventKey(formula, perception);

            //Check if the event is already contained within the agent's event catalogue
            if (EventsCatalogue.ContainsKey(the_key))
                return;

            Logger.SetColorForNextConsoleLog(ConsoleColor.Black, ConsoleColor.Cyan);
            Logger.Log(LogLevel.Debug, "[" + parentAgent.Name + "] Added event {key}" + the_key + " {plan}" + Plan.Name);

            //Add the event
            EventsCatalogue.Add(the_key, Plan);

            //Add the event's args
            EventsArgs.Add(the_key, Args);
        }

        #endregion Methods
    }
}