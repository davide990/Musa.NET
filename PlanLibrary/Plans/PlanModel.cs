using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Quartz;

namespace PlanLibrary
{
	public abstract class PlanModel : IJob
	{
		#region Fields/Properties

		/// <summary>
		/// Gets the allowed roles.
		/// </summary>
		/// <value>The allowed roles.</value>
		public HashSet<string> AllowedRoles
		{
			get { return allowed_roles; }
			private set { allowed_roles = value; }
		}
		private HashSet<string> allowed_roles;

		/// <summary>
		/// Gets the expected result.
		/// </summary>
		/// <value>The expected result.</value>
		public string ExpectedResult
		{
			get { return expected_result; }
			private set { expected_result = value; }
		}
		private string expected_result;

		/// <summary>
		/// Gets the trigger conditions.
		/// </summary>
		/// <value>The trigger conditions.</value>
		public string TriggerCondition
		{
			get { return trigger_condition; }
			private set { trigger_condition = value; }
		}
		private string trigger_condition;

		/// <summary>
		/// Gets the steps.
		/// </summary>
		/// <value>The steps.</value>
		public List<PlanStep> Steps
		{
			get { return steps;}
			private set { steps = value;}
		}
		private List<PlanStep> steps;

		/// <summary>
		/// Gets the plan name.
		/// </summary>
		public string Name
		{
			get { return GetType().Name; }
		}
		#endregion Fields/Properties

		protected PlanModel ()
		{
			AllowedRoles = new HashSet<string> ();
			Steps = new List<PlanStep> ();

			//Retrieve the plan attribute (if any)
			var plan_attribute = from t in GetType ().GetCustomAttributes (typeof(PlanAttribute), true)
			                     let attributes = t as PlanAttribute
			                     where t != null
			                     select new { 	AllowedRoles = attributes.AllowedRoles, 
												TriggerCondition = attributes.TriggerCondition,
												ExpectedResult = attributes.ExpectedResult};

			if (plan_attribute.ToList ().Count <= 0)
				throw new Exception ("Class " + GetType ().Name + " is not decorated with [Plan] attribute.");

			TriggerCondition 	= plan_attribute.ToList()[0].TriggerCondition;
			ExpectedResult 		= plan_attribute.ToList()[0].ExpectedResult;

			foreach (var v in plan_attribute) 
			{
				foreach (string role in v.AllowedRoles)
					AllowedRoles.Add (role);
			}

			//Retrieve the methods of this plan decorated with PlanStep attribute
			var plan_steps = from mm in GetType ().GetMethods (BindingFlags.Instance | BindingFlags.Public)
			                 let attribute = mm.GetCustomAttribute (typeof(PlanStepAttribute)) as PlanStepAttribute
			                 where attribute != null
			                 select new {MethodName = mm.Name, TriggerCondition = attribute.TriggerCondition};
			
			foreach (var vv in plan_steps)
				Steps.Add (new PlanStep (this, vv.MethodName, vv.TriggerCondition));
		}

		/// <summary>
		/// Execute this plan.
		/// </summary>
		public abstract void Execute (object[] args);

		/// <summary>
		/// IJob's interface method. It is hidden, and when this plan is triggered, it calls the Execute method.
		/// </summary>
		/// <param name="context">Context.</param>
		void IJob.Execute (IJobExecutionContext context)
		{
			//Execute ();
		}

	}
}

