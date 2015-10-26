using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Quartz;

namespace PlanLibrary
{
	public abstract class PlanModel
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

		/// <summary>
		/// Gets the plan entry point method's name.
		/// </summary>
		public string PlanEntryPointMethod
		{
			get { return planEntryPointMethod.Name; }
		}
		private MethodInfo planEntryPointMethod;

		public object[] Args
		{
			get { return args; }
			internal set { args = value; }
		}
		private object[] args;


		/// <summary>
		/// Initializes a new instance of the <see cref="PlanLibrary.PlanModel"/> class.
		/// </summary>
		protected PlanModel ()
		{
			AllowedRoles = new HashSet<string> ();
			Steps = new List<PlanStep> ();

			setPlanAttributes ();
			setPlanSteps ();
			setEntryPointMethod ();
		}

		/// <summary>
		/// Sets the plan attributes.
		/// </summary>
		private void setPlanAttributes()
		{
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
		}

		/// <summary>
		/// Sets the plan steps.
		/// </summary>
		private void setPlanSteps()
		{
			//Retrieve the methods of this plan decorated with PlanStep attribute
			var plan_steps = from mm in GetType ().GetMethods (BindingFlags.Instance | BindingFlags.Public)
				let attribute = mm.GetCustomAttribute (typeof(PlanStepAttribute)) as PlanStepAttribute
					where attribute != null
				select new {MethodName = mm.Name, TriggerCondition = attribute.TriggerCondition};

			foreach (var vv in plan_steps)
				Steps.Add (new PlanStep (this, vv.MethodName, vv.TriggerCondition));
		}

		/// <summary>
		/// Sets the entry point method for this plan.
		/// </summary>
		private void setEntryPointMethod()
		{
			//Search for the plan entry point method
			var entry_point = from mm in GetType ().GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				let attribute = mm.GetCustomAttribute (typeof(PlanEntryPoint)) as PlanEntryPoint
					where attribute != null
				select mm;

			//Check for entry point method count
			int entry_point_methods_count = entry_point.ToList ().Count;
			if (entry_point_methods_count < 1)
				throw new Exception ("In plan " + GetType ().Name + ": no plan entry point specified.");
			if (entry_point_methods_count > 1)
				throw new Exception ("In plan " + GetType ().Name + ": only one entry point method can be specified within a plan.");

			//Set the plan entry point method
			planEntryPointMethod = (entry_point.ToList () [0]) as MethodInfo;

			//Check for entry point method parameter
			if(planEntryPointMethod.GetParameters ().Length < 1)
				throw new Exception ("In plan " + GetType ().Name + ": entry point method must include an object[] parameter.");
			if(planEntryPointMethod.GetParameters ().Length > 1)
				throw new Exception ("In plan " + GetType ().Name + ": entry point method must include only one parameter of type object[].");
			if (!planEntryPointMethod.GetParameters () [0].ParameterType.IsEquivalentTo (typeof(object[])))
				throw new Exception ("In plan " + GetType ().Name + ": entry point method's parameter must be of type object[].");

			//set the entry point method parameter's
			//Args = planEntryPointMethod.GetParameters () [0] as object[];
		}

		internal void Execute(Object[] args)
		{
			if (planEntryPointMethod == null)
				throw new Exception ("In plan " + GetType ().Name + ": invalid entry point method.");
			
			planEntryPointMethod.Invoke (this, new object[]{ args });
		}
	}
}

