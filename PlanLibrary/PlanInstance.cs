//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  PlanInstance.cs
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
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

//using MusaLogger;
using MusaCommon;

/*using FormulaLibrary;
using FormulaLibrary.ANTLR;*/


namespace PlanLibrary
{
    /// <summary>
    /// Plan instance.
    /// </summary>
    public class PlanInstance<T> : IPlanInstance where T : PlanModel
    {
        #region Fields/Properties

        /// <summary>
        /// The trigger condition necessary to activate this plan.
        /// </summary>
        public IFormula TriggerCondition
        {
            get;
            private set;
        }

        /// <summary>
        /// The plan model this instance references to.
        /// </summary>
        private readonly PlanModel plan_model;

        /// <summary>
        /// Gets the entry point method for this plan.
        /// </summary>
        private MethodInfo EntryPointMethod
        {
            get { return plan_model.planEntryPointMethod; }
        }

        /// <summary>
        /// Gets the plan steps.
        /// </summary>
        private List<PlanStep> PlanSteps
        {
            get { return plan_model.Steps; }
        }

        /// <summary>
        /// Gets the name of this plan.
        /// </summary>
        public string Name
        {
            get { return typeof(T).Name; }
        }

        /// <summary>
        /// The background worker that handles the execution of this plan.
        /// </summary>
        private BackgroundWorker background_worker;

        /// <summary>
        /// An object used to handle pause/resume of this plan
        /// </summary>
        ManualResetEvent _busy;

        /// <summary>
        /// Gets a value indicating whether this plan has finished its execution.
        /// </summary>
        public bool HasFinished
        {
            get { return background_worker == null || !background_worker.IsBusy; }
        }

        /// <summary>
        /// An event that occurs when this plan's execution terminates.
        /// </summary>
        public event EventHandler Finished;

        /// <summary>
        /// Gets the logger of the agent this plan is registered to.
        /// </summary>
        protected ILogger Logger
        {
            get;
            private set;
        }

        #endregion Fields/Properties

        #region Events

        public delegate void onRegisterResult(string result);

        /// <summary>
        /// This event is invoked when from within the plan a result is 
        /// required to be registered into the parent agent's workbench. 
        /// </summary>
        public event onRegisterResult RegisterResult;

        #endregion Events

        #region Constructor and initialization methods

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanInstance"/>class.
        /// </summary>
        public PlanInstance()
        {
            plan_model = Activator.CreateInstance(typeof(T)) as PlanModel;
            plan_model.RegisterResultEvent += OnRegisterResult;
            plan_model.Log += Log;
		
            //add an event handler for step's execution
            foreach (PlanStep step in plan_model.Steps)
                step.ExecuteStep += onExecuteStep;

            //Parse the trigger condition of the plan model
            string trigger_condition = plan_model.TriggerCondition;

            var FormulaParser = ModuleProvider.Get().Resolve<IFormulaParser>();
            Logger = ModuleProvider.Get().Resolve<ILogger>();

            if (!string.IsNullOrEmpty(trigger_condition))
            {
                try
                {
                    TriggerCondition = FormulaParser.Parse(trigger_condition);
                }
                catch (Exception e)
                {
                    Logger.Log(LogLevel.Error, "Unable to parse plan's trigger condition '" + trigger_condition + "'.\nError: " + e.Message);
                    throw new Exception("Unable to parse plan's trigger condition '" + trigger_condition + "'.\nError: " + e.Message);
                }
            }
				
            //Initialize the ManualResetEvent object
            _busy = new ManualResetEvent(true);
        }


        private void Log(int level, string message)
        {
            Logger.Log(level, message);
        }

        /// <summary>
        /// Raised when a result has be be registered after the invocation of RegisterResult(...) within a plan step.
        /// </summary>
        /// <param name="result">The result to be registered. It is a formula.</param>
        private void OnRegisterResult(string result)
        {
            if (RegisterResult != null)
                RegisterResult(result);
            else
                throw new Exception("No agent is registered to catch this plan's results.");
        }


        /// <summary>
        /// Initializes the background worker for this plan.
        /// </summary>
        private void initializeBackgroundWorker()
        {
            background_worker = new BackgroundWorker();

            background_worker.DoWork += onBackgroundWorker_DoWork;
            background_worker.RunWorkerCompleted += onBackgroundWorker_WorkCompleted;
            background_worker.ProgressChanged += onBackgroundWorker_ProgressChanged;

            background_worker.WorkerReportsProgress = true;
            background_worker.WorkerSupportsCancellation = true;
        }

        #endregion Constructor and initialization methods

        #region Background worker methods

        void onBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //TODO log plan execution progress
        }

        void onBackgroundWorker_WorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Logger.Log(LogLevel.Trace, "[PLAN " + Name + "] Execution terminated.");

            if (Finished != null)
                Finished(this, null);
        }

        /// <summary>
        /// Execute this plan in background.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">The plan's parameters. [e.Argument] is of type Dictionary<string, object></param>
        void onBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (plan_model == null)
                throw new Exception("Error: plan model is null");

            if (EntryPointMethod == null)
                throw new Exception("In plan " + Name + ": invalid entry point method.");

            //Takes the plan's arguments
            IPlanArgs args = e.Argument as IPlanArgs;

            //If the plan step method has parameters
            if (EntryPointMethod.GetParameters().Length > 0)
            {
                //If passed args are not null
                if (args != null)
                {
                    //Check if the parameter is of type IAgentEventArgs
                    var EntryPointMethodArgType = EntryPointMethod.GetParameters()[0].ParameterType;
                    if (!(typeof(IPlanArgs).IsAssignableFrom(EntryPointMethodArgType)))
                        throw new Exception("In plan step" + Name + ": plan steps supports only a maximum of 1 parameter of type IAgentEventArgs.");

                    //Invoke the method
                    InvokePlan(new object[]{ args });
                }
                else
                {
                    //If the passed args are null, invoke the method with an empty dictionary
                    InvokePlan(new object[]{ new object() as IPlanArgs });
                }
            }
            else
            {
                //no args are passed neither provided by plan step method. Invoke the method without parameters.
                InvokePlan(null);
            }
        }

        #endregion Background worker methods


        #region Methods

        /// <summary>
        /// This is invoked when a plan step [step_name] is invoked from within the plan. This method checks if this
        /// plan is in pause or not. If in pause, wait for the execution to be resumed, then proceed executing the plan
        /// step.
        /// </summary>
        /// <param name="the_step">The plan step to be executed.</param>
        /// <param name="args">The arguments passed to the plan step (optional).</param>
        private void onExecuteStep(PlanStep the_step, IPlanArgs args = null)
        {
            //If the plan is in pause, wait until it is resumed
            _busy.WaitOne();

            //If it is not in pause, execute the requested plan step
            the_step.InvokePlanStep(args);
        }

        /// <summary>
        /// Execute this plan.
        /// </summary>
        public void Execute(IPlanArgs args = null)
        {
            if (background_worker == null)
                initializeBackgroundWorker();

            //If the plan is not already in execution, execute it
            if (!background_worker.IsBusy)
                background_worker.RunWorkerAsync(args);
            else
            {
                //TODO decidere se generare una eccezione o non fare nulla
                throw new Exception("Plan '" + Name + "' already running.");
            }
        }

        /// <summary>
        /// Pause this plan's execution.
        /// </summary>
        public void Pause()
        {
            Console.WriteLine(Name + " paused");
            _busy.Reset();
        }

        /// <summary>
        /// Resume this plan's execution.
        /// </summary>
        public void Resume()
        {
            Console.WriteLine(Name + " resumed");
            _busy.Set();
        }

        /// <summary>
        /// Abort this plan's execution.
        /// </summary>
        public void Abort()
        {
            if (!background_worker.IsBusy)
                return;
			
            _busy.Close();
            background_worker.CancelAsync();
            background_worker.Dispose();
            background_worker = null;
            GC.Collect();
        }

        /// <summary>
        /// Invokes this plan's entry point method.
        /// </summary>
        private void InvokePlan(object[] args)
        {
            try
            {
                EntryPointMethod.Invoke(plan_model, args);

                for (ushort i=0; i < plan_model.StepsOrder.Count; i++)
                {
                    var the_name = plan_model.StepsOrder[i];
                    var the_step = plan_model.Steps.Find(x => x.Name.Equals(the_name));
                    the_step.Execute();
                }
            }
            catch (Exception e)
            {
                if (e is TargetInvocationException)
                    Console.WriteLine("An exception has been throwed by the invoked plan '" + Name + "'.\nMessage: " + e.InnerException.ToString());
                else
                    Console.WriteLine("Error while executing plan " + Name + "\n" + e.ToString());
            }
        }

        /// <summary>
        /// Returns a string that represents the current <see cref="PlanLibrary.PlanInstance`1"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="PlanLibrary.PlanInstance`1"/>.</returns>
        public override string ToString()
        {
            return Name;
        }

        #region IPlanInstance inherithed methods

        public string GetName()
        {
            return Name;
        }

        public bool IsAtomic()
        {
            return plan_model.IsAtomic;
        }

        #endregion IPlanInstance inherithed methods

        #endregion Methods
    }
}

