//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  PlanModel.cs
//
//  Author:
//       Davide Guastella <davide.guastella90@gmail.com>
//
//  Copyright (c) 2016 Davide Guastella
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
using System.Linq;
using System.Reflection;
using System;
using MusaCommon;
using System.Text;

namespace PlanLibrary
{
    public abstract class PlanModel : IPlanModel
    {
        #region Fields/Properties

        /// <summary>
        /// Gets the allowed roles.
        /// </summary>
        /// <value>The allowed roles.</value>
        public HashSet<string> AllowedRoles
        {
            get;
            private set;
        }


        public Type RescuePlan
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the trigger condition for this plan model.
        /// </summary>
        /// <value>The trigger condition.</value>
        internal IFormula TriggerCondition
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the steps.
        /// </summary>
        /// <value>The steps.</value>
        public List<PlanStep> Steps
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the steps execution order. Each string is a step name.
        /// </summary>
        /// <value>The steps order.</value>
        public List<string> StepsOrder
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets this plan's name.
        /// </summary>
        public string Name
        {
            get { return GetType().Name; }
        }

        /// <summary>
        /// Gets the plan entry point method's name.
        /// </summary>
        public string EntryPointName
        {
            get { return planEntryPointMethod.Name; }
        }

        internal MethodInfo planEntryPointMethod;

        /// <summary>
        /// Gets or sets the arguments for this plan. These arguments are passed to the plan from the entry point 
        /// method.
        /// </summary>
        /// <value>The arguments.</value>
        public IPlanArgs Args
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the name of the plan step from within this property is accessed.
        /// </summary>
        public string PlanStepName
        {
            get
            { 
                MethodBase calling_method = new System.Diagnostics.StackTrace(1, false).GetFrame(0).GetMethod();
                if (calling_method.GetCustomAttributes().OfType<PlanStepAttribute>().FirstOrDefault() == null)
                    throw new Exception("Method '" + calling_method.Name + "' is not a plan step.\n");
				
                return calling_method.Name; 
            }
        }

        /// <summary>
        /// Gets a value indicating whether this plan is atomic, that is, it cannot be paused.
        /// </summary>
        public bool IsAtomic
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the workbench of the agent that owns this plan instance. This is used to
        /// test trigger conditions of the plan steps.
        /// </summary>
        /// <value>The agent workbench.</value>
        private IAgentWorkbench AgentWorkbench
        {
            get { return Parent.GetWorkbench(); }
        }

        /// <summary>
        /// Gets the source agent which communicated a knowledge for which this plan has been invoked (as result of an event trigger).
        /// It is analogous to [source(X)] annotation in Jason/Agentspeak
        /// </summary>
        /// <value>The source agent.</value>
        public AgentPassport SourceAgent
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the parent agent. Plans can be nested into agent classes, so this member provide an helpful way to access to parent agent
        /// </summary>
        /// <value>The parent.</value>
        protected IAgent Parent
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        protected ILogger Logger
        {
            get { return ModuleProvider.Get().Resolve<ILogger>(); }
        }

        #endregion Fields/Properties

        #region Events

        internal delegate void onRegisterResult(string result);

        /// <summary>
        /// Used to register results into agent's workbenches
        /// </summary>
        internal event onRegisterResult RegisterResultEvent;


        internal delegate void onLog(int level,string message);

        internal event onLog Log;

        #endregion Events

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanLibrary.PlanModel"/> class.
        /// </summary>
        protected PlanModel()
        {
            AllowedRoles = new HashSet<string>();
            Steps = new List<PlanStep>();
            StepsOrder = new List<string>();

            try
            {
                checkAtomicPlan();
                parseAttributes();
                setPlanSteps();
                setEntryPointMethod();
            }
            catch (Exception e)
            {
                //TODO logger necessario qui
                Console.WriteLine(e.ToString());
            }
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Checks if this plan is decorated with [AtomicPlan] attribute.
        /// </summary>
        private void checkAtomicPlan()
        {
            IsAtomic = GetType().GetCustomAttributes(typeof(AtomicPlanAttribute), true).ToList().Count > 0;
        }

        /// <summary>
        /// Sets the plan attributes.
        /// </summary>
        private void parseAttributes()
        {
            parsePlanAttribute();
            parsePlanStepsOrderAttribute();
        }

        private void parsePlanAttribute()
        {
            //Retrieve the plan attribute (if any)
            var plan_attribute = from t in GetType().GetCustomAttributes(typeof(PlanAttribute), true)
                                          let attributes = t as PlanAttribute
                                          where t != null
                                          select new {    AllowedRoles = attributes.AllowedRoles, 
                TriggerCondition = attributes.TriggerCondition,
                RescuePlan = attributes.RescuePlan};
            
            //Raise an exception if the class/plan is not decorated with [Plan] attribute
            if (plan_attribute.ToList().Count <= 0)
                throw new Exception("Class " + GetType().Name + " is not decorated with [Plan] attribute.");

            //Get the trigger condition
            var triggerCondition = plan_attribute.ToList()[0].TriggerCondition;

            //Convert the string trigger condition to a IFormula object
            parseTriggerCondition(triggerCondition);

            //Get the expected result
            RescuePlan = plan_attribute.ToList()[0].RescuePlan;

            foreach (var v in plan_attribute)
            {
                foreach (string role in v.AllowedRoles)
                    AllowedRoles.Add(role);
            }
        }

        /// <summary>
        /// Parses the trigger condition provided in this plan model's attribute, and convert it from
        /// string to a IFormula object.
        /// </summary>
        private void parseTriggerCondition(string triggerCondition)
        {
            if (string.IsNullOrEmpty(triggerCondition))
                return;

            var fp = ModuleProvider.Get().Resolve<IFormulaUtils>();
            if (fp == null)
                throw new Exception("Unable to parse trigger condition '" + triggerCondition + "' in plan '" + Name + "'. Formula Parser not avaible.\n");

            TriggerCondition = fp.Parse(triggerCondition);
        }

        private void parsePlanStepsOrderAttribute()
        {
            //Get the steps order from the attribute PlanStepOrder
            var steps_order = from t in GetType().GetCustomAttributes(typeof(PlanStepsOrderAttribute), true)
                                       let attributes = t as PlanStepsOrderAttribute
                                       where t != null
                                       select new {    StepsOrder = attributes.StepsName};

            //Check if actually any step order has been specified
            if (steps_order.ToList().Count <= 0)
                return;

            //then, add the order to a list
            foreach (var v in steps_order.First().StepsOrder)
                StepsOrder.Add(v);
        }

        /// <summary>
        /// Sets the plan steps.
        /// </summary>
        private void setPlanSteps()
        {
            //Retrieve the methods of this plan decorated with PlanStep attribute
            var plan_steps = from mm in GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                      let attribute = mm.GetCustomAttribute(typeof(PlanStepAttribute)) as PlanStepAttribute
                                      where attribute != null
                                      select new {Method = mm, TriggerCondition = attribute.TriggerCondition};

            var fp = ModuleProvider.Get().Resolve<IFormulaUtils>();
            if (fp == null)
                throw new Exception("In plan '" + Name + "': Formula Parser not avaible.\n");
            
            foreach (var vv in plan_steps)
            {
                if (string.IsNullOrEmpty(vv.TriggerCondition))
                    Steps.Add(new PlanStep(this, vv.Method));
                else
                    Steps.Add(new PlanStep(this, vv.Method, fp.Parse(vv.TriggerCondition)));
            }
        }

        /// <summary>
        /// Sets the entry point method for this plan.
        /// </summary>
        private void setEntryPointMethod()
        {
            //Search for the plan entry point method
            var entry_point = from mm in GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                       let attribute = mm.GetCustomAttribute(typeof(PlanEntryPoint)) as PlanEntryPoint
                                       where attribute != null
                                       select mm;

            //Check for entry point method count
            int entry_point_methods_count = entry_point.ToList().Count;
            if (entry_point_methods_count < 1)
                throw new Exception("In plan " + GetType().Name + ": no plan entry point specified.");
            if (entry_point_methods_count > 1)
                throw new Exception("In plan " + GetType().Name + ": only one entry point method can be specified within a plan.");

            //Set the plan entry point method
            planEntryPointMethod = (entry_point.ToList()[0]) as MethodInfo;

            //Check for entry point method parameter
            if (planEntryPointMethod.GetParameters().Length < 1)
                throw new Exception("In plan " + GetType().Name + ": entry point method '" + planEntryPointMethod.Name + "' must include a parameter of type " + typeof(IPlanArgs).Name);
            if (planEntryPointMethod.GetParameters().Length > 1)
                throw new Exception("In plan " + GetType().Name + ": entry point method '" + planEntryPointMethod.Name + "' must include only one parameter of type " + typeof(IPlanArgs).Name);

            var entryPointFirstArgType = planEntryPointMethod.GetParameters()[0].ParameterType;

            //if (!.IsEquivalentTo(typeof(IAgentEventArgs)))
            if (!(typeof(IPlanArgs).IsAssignableFrom(entryPointFirstArgType)))
                throw new Exception("In plan " + GetType().Name + ": entry point method's parameter must be of type " + typeof(IPlanArgs).Name);
        }

        /// <summary>
        /// Executes a plan step.
        /// </summary>
        /// <param name="step_name">The step name to be executed. It is the name of a method decorated with
        /// [PlanStep] atttribute.</param>
        /// <param name="args">The step arguments.</param>
        public void ExecuteStep(string step_name, IPlanArgs args = null)
        {
            //Step execution is ignored if an execution order has been specified
            if (StepsOrder.Count > 0)
            {
                log(LogLevel.Warn, " In plan '" + Name + "', ExecuteStep(" + step_name + ") has been ignored since plan has a steps execution order specified.");
                return;
            }

            bool plan_found = false;

            foreach (PlanStep step in Steps)
            {
                if (!step.Name.Equals(step_name))
                    continue;

                plan_found = true;

                if (step.TriggerCondition != null)
                {
                    List<IAssignment> generatedAssignment;
                    bool test_condition = AgentWorkbench.TestCondition(step.TriggerCondition, out generatedAssignment);
                    if (!test_condition)
                    {
                        Log(LogLevel.Error, "Plan step'" + step.Name + "' cannot be executed: trigger condition '" + step.TriggerCondition + "' not satisfied in agent belief base.");
                        return;
                    }

                    var sb = new StringBuilder();
                    sb.Append("Condition '" + step.TriggerCondition + "' satisfied in agent's workbench. Generated assignments :");
                    generatedAssignment.ForEach(x => sb.Append(x + " "));
                    Log(LogLevel.Trace, sb.ToString());
                }

                //TODO log internal event?
                //Internal event
                step.Execute(args);
            }

            //Throw an exception if no plan step named [plan_step] has been found
            if (!plan_found)
                throw new Exception("In plan " + Name + ": cannot find plan step '" + step_name + "'");
        }

        protected void ExecuteExternalPlan()
        {
            // TODO implementare invocazione piani esterni (cioè in ambienti remoti)
        }

        /// <summary>
        /// Registers a result to the workbench of the agent that executes this plan.
        /// </summary>
        /// <param name="result">The result to be registered. It is a formula.</param>
        public void RegisterResult(string result)
        {
            RegisterResultEvent(result);
        }

        internal void SetSourceAgent(AgentPassport source)
        {
            SourceAgent = source;
        }

        #endregion Methods

        public string GetEntryPointName()
        {
            return EntryPointName;
        }

        public string GetPlanName()
        {
            return Name;
        }

        public string GetPlanStepName()
        {
            return PlanStepName;
        }

        internal void SetParent(IAgent parent)
        {
            Parent = parent;
        }

        internal IAgent GetParent()
        {
            return Parent;
        }

        /// <summary>
        /// Log the specified message.
        /// </summary>
        /// <param name="LogLevel">Log level.</param>
        /// <param name="message">Message.</param>
        public void log(int LogLevel, string message)
        {
            //Raise an event that will be catched from the PlanInstance instance this model is related to. As it's 
            //catched, the PlanInstance's logger will log the requested message.
            if (Log != null)
                Log(LogLevel, message);
        }

        public Type GetRescuePlan()
        {
            return RescuePlan;
        }

    }
}

