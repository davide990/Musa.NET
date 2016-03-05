using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using MusaCommon;

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

        /// <summary>
        /// Gets the expected result.
        /// </summary>
        /// <value>The expected result.</value>
        public string ExpectedResult
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
                ExpectedResult = attributes.ExpectedResult};
            
            //Raise an exception if the class/plan is not decorated with [Plan] attribute
            if (plan_attribute.ToList().Count <= 0)
                throw new Exception("Class " + GetType().Name + " is not decorated with [Plan] attribute.");

            //Get the trigger condition
            var triggerCondition = plan_attribute.ToList()[0].TriggerCondition;

            //Convert the string trigger condition to a IFormula object
            parseTriggerCondition(triggerCondition);

            //Get the expected result
            ExpectedResult = plan_attribute.ToList()[0].ExpectedResult;

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

            var fp = ModuleProvider.Get().Resolve<IFormulaParser>();
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

            foreach (var vv in plan_steps)
                Steps.Add(new PlanStep(this, vv.Method, vv.TriggerCondition));
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

        /// <summary>
        /// Log the specified message.
        /// </summary>
        /*protected void log(LogLevel level, string message)
        {
            //Raise an event that will be catched from the PlanInstance instance this model is related to. As it's 
            //catched, the PlanInstance's logger will log the requested message.
            if (Log != null)
                Log(level, message);
        }*/

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


    }
}

