using System;
using Quartz;
using System.Reflection;
using System.Linq;
using FormulaLibrary;
using FormulaLibrary.ANTLR;
using System.Collections.Generic;
using System.ComponentModel;

namespace PlanLibrary
{
	public interface IPlanInstance
	{}

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
		/// The background worker that handles the execution of this plan.
		/// </summary>
		private BackgroundWorker background_worker;

		#endregion Fields/Properties

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="PlanInstance"/>class.
		/// </summary>
		public PlanInstance ()
		{
			//Generate a unique key for this plan instance
			PlanKey = new JobKey (typeof(T).Name).ToString();

			plan_model = Activator.CreateInstance (typeof(T)) as PlanModel;

			//Parse the trigger condition of the plan model
			string trigger_condition = plan_model.TriggerCondition;

			//TriggerCondition = FormulaParser.Parse (trigger_condition);
		}

		#endregion Constructor

		#region Methods

		/// <summary>
		/// Execute this plan.
		/// </summary>
		public void Execute(Dictionary<string,object> args = null)
		{
			if(plan_model == null)
				throw new Exception ("Error: plan model is null");

			if (EntryPointMethod == null)
				throw new Exception ("In plan " + plan_model.Name + ": invalid entry point method.");
			
			//If the plan step method has parameters
			if (EntryPointMethod.GetParameters ().Length > 0) 
			{
				//If passed args are not null
				if (args != null) 
				{
					//Check if the parameter is of type Dictionary<string,object>
					if (!EntryPointMethod.GetParameters () [0].ParameterType.IsEquivalentTo (typeof(Dictionary<string,object>)))
						throw new Exception ("In plan step" + plan_model.Name + ": plan steps supports only a maximum of 1 parameter of type Dictionary<string,object>.");

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

		/// <summary>
		/// Invokes this plan's entry point.
		/// </summary>
		private void InvokePlan(object[] args)
		{
			try
			{
				EntryPointMethod.Invoke (plan_model, args);
			}
			catch(TargetInvocationException e)
			{
				Console.WriteLine ("An exception has been throwed by the invoked plan '"+plan_model.Name+"'.\nMessage: "+e.InnerException.ToString());
			}
		}

		#endregion Methods
	}
}

