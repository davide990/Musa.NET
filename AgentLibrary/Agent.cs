//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  Agent.cs
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

using System;
using System.Collections.Generic;
using System.Collections;
using AgentLibrary;
using System.Reflection;
using FormulaLibrary;
using MusaCommon;

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
        /// The belief set of this agent.
        /// </summary>
        public List<AtomicFormula> Beliefs
        {
            get{ return new List<AtomicFormula>(Workbench.Statements); }
        }

        public List<AssignmentType> Assignments
        {
            get { return new List<AssignmentType>(Workbench.AssignmentSet); }
        }

        /// <summary>
        /// This queue contains the changes to the environment that this agent have to perceive. Since an agent can be 
        /// busy in doing other activities such event-handling or execution of plans, the perceived environment changes 
        /// are accumulated temporarily in this queue until the agent start a perception activity.
        /// The key contains the formula(s) to be perceived (AtomicFormula type).
        /// 
        /// Ogni percezione può coinvolgere più di una singola credenza
        /// </summary>
        public Stack<Tuple<IList, AgentPerception>> PerceivedEnvironementChanges
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

        private Stack<Tuple<IList, AgentPerception>> perceivedEnvironementChanges;
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
                lock (lock_mailBox)
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
        private IPlanCollection PlansCollection;

        /// <summary>
        /// Gets the events.
        /// </summary>
        /// <value>The events.</value>
        public Dictionary<AgentEventKey, Type> Events
        {
            get { return reasoner.EventsCatalogue; }
        }

        /// <summary>
        /// Gets the events arguments.
        /// </summary>
        /// <value>The events arguments.</value>
        public Dictionary<AgentEventKey, AgentEventArgs> EventsArgs
        {
            get { return reasoner.EventsArgs; }
        }

        /// <summary>
        /// Gets a value indicating whether this agent is busy (executing a plan).
        /// </summary>
        public bool Busy
        {
            get { return agent_is_busy; }
            private set { agent_is_busy = value; }
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
            get;
            internal set;
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
            get { return reasoner.IsRunning; }
        }

        /// <summary>
        /// The working end time of this agent
        /// </summary>
        public DateTime WorkScheduleEnd
        {
            get;
            private set;
        }

        /// <summary>
        /// The working start time of this agent
        /// </summary>
        public DateTime WorkScheduleStart
        {
            get;
            private set;
        }

        /// <summary>
        /// The role of this agent
        /// </summary>
        public string Role
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether this agent is paused.
        /// </summary>
        public bool Paused
        {
            get;
            private set;
        }

        #endregion

        /// <summary>
        /// The logger of this agent. It is automatically configured using the environment configuration file. If this
        /// file is missing, a console logger is automatically set up.
        /// </summary>
        public ILogger Logger
        {
            get;
            private set;
        }

        /// <summary>
        /// The "pointer" to the method invoked when a plan of this agent 
        /// requires to register a result.
        /// </summary>
        private MethodInfo RegisterResultHandler
        {
            get;
            set;
        }

        /// <summary>
        /// The "pointer" to the method invoked when a plan of this agent 
        /// terminates its execution
        /// </summary>
        private MethodInfo PlanFinishedHandler
        {
            get;
            set;
        }

        /// <summary>
        /// An utility class for handling plans
        /// </summary>
        private IPlanFacade PlanFacade;

        #region Constructors

        /// <summary>
        /// Create a new agent
        /// </summary>
        public Agent()
            : this("agent_" + (new Random().Next(Int32.MaxValue)))
        {
        }

        /// <summary>
        /// Create a new agent
        /// </summary>
        /// <param name="agent_name"></param>
        public Agent(string agent_name)
        {
            if (string.IsNullOrEmpty(agent_name))
                name = "agent_" + (new Random().Next(Int32.MaxValue));
            else
                name = agent_name;

            pause_requested = false;
            ID = Guid.NewGuid();
            roles = new List<AgentRole>();
            Workbench = new AgentWorkbench(this);
            reasoner = new AgentReasoner(this);
            PerceivedEnvironementChanges = new Stack<Tuple<IList, AgentPerception>>();
            mailBox = new Stack<Tuple<AgentPassport, AgentMessage>>();
            createdAt = DateTime.Now;
            resume_reasoning = false;

            //Inject the logger
            Logger = ModuleProvider.Get().Resolve<ILogger>();

            //Inject the PlanFacade 
            PlanFacade = ModuleProvider.Get().Resolve<IPlanFacade>();

            //Create a PlanCollection object
            PlansCollection = PlanFacade.CreatePlanCollection();

            RegisterResultHandler = GetType().GetMethod("onPlanInstanceRegisterResult", BindingFlags.NonPublic | BindingFlags.Instance);
            PlanFinishedHandler = GetType().GetMethod("onPlanInstanceFinished", BindingFlags.NonPublic | BindingFlags.Instance);

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
            ps.AgentName = Name;
            ps.AgentRole = Role;
            ps.EnvironementAddress = EnvironementIPAddress;
            return ps;
        }

        /// <summary>
        /// Notify to this agent a change occurred within the environment this 
        /// agent is located.
        /// </summary>
        /// <param name="action">The type of change occurred</param>
        /// <param name="changes">The data that involved in the environment 
        /// change. Each element of [changes] is a formula to be added to this 
        /// agent's workbench</param>
        internal void notifyEnvironementChanges(AgentPerception action, IList changes)
        {
            foreach (var v in changes)
                Logger.Log(LogLevel.Trace, "[" + name + "] received " + action + " -> " + v);
            
            PerceivedEnvironementChanges.Push(new Tuple<IList, AgentPerception>(changes, action));
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
            if (current_executing_plan.IsAtomic())
            {
                Logger.SetColorForNextConsoleLog(ConsoleColor.Black, ConsoleColor.DarkYellow);
                Logger.Log(LogLevel.Trace, "[" + Name + "] I'm executing an atomic plan. It Will be paused after the plan has finished its execution.");

                //Request this agent to pause only after the current atomic plan 
                //has finished its execution
                pause_requested = true;
                return;
            }

            //Set the pause state 
            Paused = true;

            //Pause the agent's reasoner
            reasoner.Pause();

            //Suspend the current executing plan
            CurrentExecutingPlan.Pause();

            Logger.Log(LogLevel.Trace, "[" + Name + "] paused");
        }

        /// <summary>
        /// Resume this agent's execution.
        /// </summary>
        public void Resume()
        {
            if (!Paused)
                return;
			
            Paused = false;

            Logger.SetColorForNextConsoleLog(ConsoleColor.Black, ConsoleColor.DarkYellow);
            Logger.Log(LogLevel.Trace, "[" + Name + "] resumed");

            //Resume the execution of the suspended plan
            CurrentExecutingPlan.Resume();

            if (CurrentExecutingPlan == null)
                reasoner.Resume();
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
        /// <param name="PlanModel">The plan model to be added. An instance of this plan is instantiated</param>
        public void AddPlan(Type PlanModel)
        {
            //Check if the provisioned type is a valid plan
            if (!(typeof(IPlanModel).IsAssignableFrom(PlanModel)))
                throw new Exception("Argument #1 in AddPlan(Type) must implement IPlanModel.");
            
            //Create a new plan instance
            var plan_instance = PlanFacade.CreatePlanInstance(PlanModel, this, RegisterResultHandler, PlanFinishedHandler, Logger);

            //Add the plan to the agent's plan collection
            PlansCollection.Add(PlanModel, plan_instance);
        }

        /// <summary>
        /// Adds the plan.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="PlanName">Plan name.</param>
        public void AddPlan(Assembly assembly, string PlanName)
        {
            if (assembly.IsDefined(assembly.GetType(PlanName)))
                return;

            Type Plan = assembly.GetType(PlanName);

            if (!Plan.BaseType.IsEquivalentTo(typeof(IPlanModel)))
                throw new Exception("In AddPlan(Assembly, string): the specified" +
                    "plan '" + PlanName + "' must implement IPlanModel.");

            //Create a new plan instance
            var plan_instance = PlanFacade.CreatePlanInstance(Plan, this, RegisterResultHandler, PlanFinishedHandler, Logger);

            //Add the plan to the agent's plan collection
            PlansCollection.Add(Plan, plan_instance);
        }

        #region Agent's plans event handlers

        /// <summary>
        /// Invoked when a plan executed by this agent terminates its execution.
        /// </summary>
        /// <param name="sender">The executed plan.</param>
        /// <param name="args">Arguments.</param>
        private void onPlanInstanceFinished(object sender, EventArgs args)
        {
            Logger.SetColorForNextConsoleLog(ConsoleColor.Black, ConsoleColor.DarkYellow);
            Logger.Log(LogLevel.Trace, "[" + Name + "] plan " + (sender as IPlanInstance).GetName() + " finished its execution.");

            //Set the value of CurrentExecutingPlan to null
            CurrentExecutingPlan = null;

            //Set the agent busy state to false
            Busy = false;

            //Check if a pause has been requested
            if (pause_requested)
            {
                //Set the pause state 
                Paused = true;

                pause_requested = false;

                //Pause the agent's reasoner
                reasoner.Pause();

                return;
            }

            if (resume_reasoning)
            {
                reasoner.Resume();
                resume_reasoning = false;
            }
        }

        /// <summary>
        /// Invoked when a plan requests to register a result.
        /// </summary>
        /// <param name="result">Result.</param>
        private void onPlanInstanceRegisterResult(string result)
        {
            Formula resultFormula = FormulaParser.Parse(result);


            if (resultFormula != null)
            {
                Workbench.AddStatement(resultFormula);

                //TODO log register result
                Logger.Log(LogLevel.Trace, "[" + Name + "] registered result: " + result);
            }
        }

        #endregion Agent's plans event handlers

        /// <summary>
        /// Executes a plan.
        /// </summary>
        /// <param name="PlanModel">Plan.</param>
        /// <param name="args">Arguments.</param>
        internal void ExecutePlan(Type PlanModel, IAgentEventArgs args = null)
        {
            if (Busy)
                throw new Exception("Agent [" + Name + "] is currently executing plan " + CurrentExecutingPlan);
            	
            //If Plan doesn't implement IPlanModel, then throw an exception
            if (!(typeof(IPlanModel).IsAssignableFrom(PlanModel)))
                throw new Exception("Argument #1 in ExecutePlan(Type) must implement IPlanModel.");
			
            IPlanInstance the_plan = null;

            //Search for the input plan
            PlansCollection.TryGetValue(PlanModel, out the_plan);

            //If the input plan has not been found within the agent's plans collection, throw an exception
            if (the_plan == null)
                throw new Exception("Plan '" + PlanModel.Name + "' not found in agent [" + Name + "] plans collection.");

            //Get the plan's Execute() method
            MethodInfo execute_method = PlanFacade.GetExecuteMethodForPlan(PlanModel);

            //Set the agent to busy
            Busy = true;

            //Set the value for CurrentExecutingPlan 
            CurrentExecutingPlan = the_plan;

            //Execute the plan
            execute_method.Invoke(the_plan, new object[]{ args });

            //TODO [ALTA PRIORITÀ] migliorare il sistema di bloccaggio dell'agente
            //Lock the agent's reasoning life cycle until the invoked plan terminates its execution
            while (Busy)
                ;
        }

        /// <summary>
        /// Tell this agent to achieve a goal by executing a specified plan
        /// </summary>
        public void AchieveGoal(Type Plan, AgentEventArgs Args = null)
        {
            if (Plan == null)
                throw new ArgumentNullException("Plan", "Argument #1 'Plan' cannot be null.");

            //Set the achievement of the plan [Plan] as an agent's perception
            PerceivedEnvironementChanges.Push(new Tuple<IList, AgentPerception>(new List<Type>{ Plan }, AgentPerception.Achieve));

            //Set the parameters to be used when the plan [Plan] is invoked
            //TODO ATTENZIONE QUI
            if (Args != null)
                reasoner.EventsArgs.Add(new AgentEventKey(Plan.Name, AgentPerception.Achieve), Args);
        }

        /// <summary>
        /// Adds an event.
        /// </summary>
        /// <param name="formula">The formula to reason on.</param>
        /// <param name="perception">The perception this event reacts to.</param>
        /// <param name="Plan">The plan to be invoked when the event is
        /// triggered.</param>
        /// <param name="Args">The argument to be passed to the invoked plan
        /// when the event is triggered.</param>
        public void AddEvent(string formula, AgentPerception perception, Type Plan, AgentEventArgs Args = null)
        {
            reasoner.AddEvent(formula, perception, Plan, Args);
        }

        /// <summary>
        /// Adds an event.
        /// </summary>
        /// <param name="ev">The event to be added</param>
        public void AddEvent(AgentEvent ev)
        {
            reasoner.AddEvent(ev.Formula, ev.Perception, ev.Plan, ev.Args);
        }

        /// <summary>
        /// Adds the beliefs to this agent's workbench. Each formula is added 
        /// as a unique agent perception.
        /// </summary>
        public void AddBelief(params AtomicFormula[] formula)
        {
            foreach (AtomicFormula ff in formula)
            {
                Logger.SetColorForNextConsoleLog(ConsoleColor.Black, ConsoleColor.Magenta);
                Logger.Log(LogLevel.Debug, "[" + Name + "] Adding belief " + ff);
                PerceivedEnvironementChanges.Push(new Tuple<IList, AgentPerception>(new List<AtomicFormula>{ ff }, AgentPerception.AddBelief));
            }
        }

        public void AddBelief(params Formula[] formula)
        {
            foreach (Formula af in formula)
            {
                Logger.SetColorForNextConsoleLog(ConsoleColor.Black, ConsoleColor.Magenta);
                Logger.Log(LogLevel.Debug, "[" + Name + "] Adding belief " + af);
                PerceivedEnvironementChanges.Push(new Tuple<IList, AgentPerception>(FormulaUtils.UnrollFormula(af), AgentPerception.AddBelief));
            }
        }

        public void AddBelief(IList formula_list)
        {
            foreach (Formula af in formula_list)
            {
                Logger.SetColorForNextConsoleLog(ConsoleColor.Black, ConsoleColor.Magenta);
                Logger.Log(LogLevel.Debug, "[" + Name + "] Adding belief " + af);
                PerceivedEnvironementChanges.Push(new Tuple<IList, AgentPerception>(FormulaUtils.UnrollFormula(af), AgentPerception.AddBelief));
            }
        }


        #endregion

        public bool TestCondition(Formula formula)
        {
            return Workbench.TestCondition(formula);
        }

    }
}