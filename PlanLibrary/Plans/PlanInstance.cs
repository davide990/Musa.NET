using System;
using System.Reflection;
using System.Linq;
using FormulaLibrary;
using FormulaLibrary.ANTLR;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace PlanLibrary
{
	public interface IPlanInstance
	{
		/// <summary>
		/// Gets the name of this plan.
		/// </summary>
		string GetName();
	}

	/// <summary>
	/// Plan instance.
	/// </summary>
	public class PlanInstance<T> : IPlanInstance where T : PlanModel
	{
		#region Fields/Properties

		/// <summary>
		/// The unique key for this plan instance
		/// </summary>
		public string PlanKey
		{
			get { return plan_key; }
			private set { plan_key = value; }
		}
		private string plan_key;

		/// <summary>
		/// The trigger condition necessary to activate this plan.
		/// </summary>
		public Formula TriggerCondition
		{
			get { return triggerCondition; }		//Type of TriggerCondition here must be Formula
			private set { triggerCondition = value; }
		}
		private Formula triggerCondition;

		/// <summary>
		/// The plan model this instance references to.
		/// </summary>
		private PlanModel plan_model;

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
			get { return background_worker == null ? true : !background_worker.IsBusy; }
		}

		/// <summary>
		/// An event that occurs when this plan's execution terminates.
		/// </summary>
		public event EventHandler Finished;

		#endregion Fields/Properties


		public delegate void onRegisterResult(string result);
		/// <summary>
		/// Used to register results into agent's workbenches
		/// </summary>
		public event onRegisterResult RegisterResult;


		#region Constructor and initialization methods

		/// <summary>
		/// Initializes a new instance of the <see cref="PlanInstance"/>class.
		/// </summary>
		public PlanInstance ()
		{
			//Generate a unique key for this plan instance
			//TODO PlanKey = new JobKey (typeof(T).Name).ToString();

			plan_model = Activator.CreateInstance (typeof(T)) as PlanModel;
			plan_model.RegisterResultEvent += OnRegisterResult;
				
			//add an event handler for step's execution
			foreach(PlanStep step in plan_model.Steps)
				step.ExecuteStep += onExecuteStep;

			//Parse the trigger condition of the plan model
			string trigger_condition = plan_model.TriggerCondition;

			//TODO parse trigger condition formula here
			//TriggerCondition = FormulaParser.Parse (trigger_condition);

			_busy = new ManualResetEvent (true);
		}

		/// <summary>
		/// Raised when a result has be be registered after the invocation of RegisterResult(...) within a plan step.
		/// </summary>
		/// <param name="result">Result.</param>
		private void OnRegisterResult (string result)
		{
			if(RegisterResult != null) 
			{
				//TODO log result to logger
				//background_worker.ReportProgress(0,result);
				RegisterResult (result);
			}
			else
				throw new Exception("No agent is registered to catch this plan's results.");
		}


		/// <summary>
		/// Initializes the background worker for this plan.
		/// </summary>
		private void initializeBackgroundWorker()
		{
			background_worker = new BackgroundWorker ();

			background_worker.DoWork 				+= onBackgroundWorker_DoWork;
			background_worker.RunWorkerCompleted 	+= onBackgroundWorker_WorkCompleted;
			background_worker.ProgressChanged 		+= onBackgroundWorker_ProgressChanged;

			background_worker.WorkerReportsProgress = true;
			background_worker.WorkerSupportsCancellation = true;
		}
		#endregion Constructor and initialization methods

		#region Background worker methods

		void onBackgroundWorker_ProgressChanged (object sender, ProgressChangedEventArgs e)
		{
			//TODO log plan execution progress
		}

		void onBackgroundWorker_WorkCompleted (object sender, RunWorkerCompletedEventArgs e)
		{
			//TODO log plan execution complete

			if (Finished != null)
				Finished (this, null);
		}

		/// <summary>
		/// Execute this plan in background.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">The plan's parameters. [e.Argument] is of type Dictionary<string, object></param>
		void onBackgroundWorker_DoWork (object sender, DoWorkEventArgs e)
		{
			if(plan_model == null)
				throw new Exception ("Error: plan model is null");

			if (EntryPointMethod == null)
				throw new Exception ("In plan " + Name + ": invalid entry point method.");

			//Takes the plan's arguments
			Dictionary<string, object> args = e.Argument as Dictionary<string, object>;

			//If the plan step method has parameters
			if (EntryPointMethod.GetParameters ().Length > 0)
			{
				//If passed args are not null
				if (args != null) 
				{
					//Check if the parameter is of type Dictionary<string,object>
					if (!EntryPointMethod.GetParameters () [0].ParameterType.IsEquivalentTo (typeof(Dictionary<string,object>)))
						throw new Exception ("In plan step" + Name + ": plan steps supports only a maximum of 1 parameter of type Dictionary<string,object>.");

					//Invoke the method
					InvokePlan (new object[]{ args });
				} 
				else 
				{
					//If the passed args are null, invoke the method with an empty dictionary
					InvokePlan (new object[]{ new Dictionary<string, object> () });
				}
			} 
			else 
			{
				//no args are passed neither provided by plan step method. Invoke the method without parameters.
				InvokePlan (null);
			}

		}

		#endregion Background worker methods

		#region Methods

		/// <summary>
		/// This is invoked when a plan step [step_name] is invoked from within the plan. This method checks if this
		/// plan is in pause or not. If in pause, wait for the execution to be resumed, then proceed executing the plan
		/// step.
		/// </summary>
		/// <param name="step_name">The plan step to be executed.</param>
		/// <param name="args">The arguments passed to the plan step (optional).</param>
		private void onExecuteStep (PlanStep the_step, Dictionary<string, object> args = null)
		{
			//If the plan is in pause, wait until it is resumed
			_busy.WaitOne ();

			//If it is not in pause, execute the requested plan step
			the_step.InvokePlanStep(args);
		}

		/// <summary>
		/// Execute this plan.
		/// </summary>
		public void Execute(Dictionary<string,object> args = null)
		{
			if(background_worker == null)
				initializeBackgroundWorker ();

			//If the plan is not already in execution, execute it
			if (!background_worker.IsBusy) 
			{
				background_worker.RunWorkerAsync (args);
			}
			else
			{
				//TODO decidere se generare una eccezione o non fare nulla
				//Else, ...
				throw new Exception("Plan '"+Name+"' already running.");
			}
		}

		/// <summary>
		/// Pause this plan's execution.
		/// </summary>
		public void Pause()
		{
			Console.WriteLine (Name + " paused");
			_busy.Reset ();
		}

		/// <summary>
		/// Resume this plan's execution.
		/// </summary>
		public void Resume()
		{
			Console.WriteLine (Name + " resumed");
			_busy.Set();
		}

		/// <summary>
		/// Abort this plan's execution.
		/// </summary>
		public void Abort()
		{
			if (!background_worker.IsBusy)
				return;
			
			_busy.Close ();
			background_worker.CancelAsync ();
			background_worker.Dispose ();
			background_worker = null;
			GC.Collect ();
		}

		/// <summary>
		/// Invokes this plan's entry point method.
		/// </summary>
		private void InvokePlan(object[] args)
		{
			try
			{
				EntryPointMethod.Invoke (plan_model, args);
			}
			catch(Exception e)
			{
				if (e is TargetInvocationException)
					Console.WriteLine ("An exception has been throwed by the invoked plan '" + Name + "'.\nMessage: " + e.InnerException.ToString ());
				else
					Console.WriteLine (e.ToString ());
			}
		}

		#region IPlanInstance inherithed methods

		public string GetName()
		{
			return Name;
		}

		#endregion IPlanInstance inherithed methods

		#endregion Methods
	}
}

