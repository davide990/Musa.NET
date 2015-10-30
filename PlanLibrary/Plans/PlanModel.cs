using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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
		public Dictionary<string,object> Args
		{
			get { return args; }
			internal set { args = value; }
		}
		private Dictionary<string,object> args;

		/// <summary>
		/// Gets the name of the plan step from within this property is accessed.
		/// </summary>
		public string PlanStepName
		{
			get 
			{ 
				MethodBase calling_method = new System.Diagnostics.StackTrace (1, false).GetFrame (0).GetMethod ();
				if (calling_method.GetCustomAttributes().OfType<PlanStepAttribute>().FirstOrDefault() == null)
					throw new Exception ("Method '" + calling_method.Name + "' is not a plan step.\n");
				
				return calling_method.Name; 
			}
		}

		#endregion Fields/Properties

		internal delegate void onRegisterResult(string result);
		/// <summary>
		/// Used to register results into agent's workbenches
		/// </summary>
		internal event onRegisterResult RegisterResultEvent;



		/// <summary>
		/// Initializes a new instance of the <see cref="PlanLibrary.PlanModel"/> class.
		/// </summary>
		protected PlanModel ()
		{
			AllowedRoles = new HashSet<string> ();
			Steps = new List<PlanStep> ();

			try
			{
				parsePlanAttributes ();
				setPlanSteps ();
				setEntryPointMethod ();
			}
			catch(Exception e)
			{
				Console.WriteLine (e.Message);
			}
		}

		/// <summary>
		/// Sets the plan attributes.
		/// </summary>
		private void parsePlanAttributes()
		{
			//Retrieve the plan attribute (if any)
			var plan_attribute = from t in GetType ().GetCustomAttributes (typeof(PlanAttribute), true)
			                     let attributes = t as PlanAttribute
			                     where t != null
			                     select new { 	AllowedRoles 		= attributes.AllowedRoles, 
												TriggerCondition 	= attributes.TriggerCondition,
												ExpectedResult 		= attributes.ExpectedResult};

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
			var plan_steps = from mm in GetType ().GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
			                 let attribute = mm.GetCustomAttribute (typeof(PlanStepAttribute)) as PlanStepAttribute
			                 where attribute != null
			                 select new {Method = mm, TriggerCondition = attribute.TriggerCondition};

			foreach (var vv in plan_steps)
				Steps.Add (new PlanStep (this, vv.Method, vv.TriggerCondition));
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
				throw new Exception ("In plan " + GetType ().Name + ": entry point method '"+planEntryPointMethod.Name+"' must include a Dictionary<string,object> parameter.");
			if(planEntryPointMethod.GetParameters ().Length > 1)
				throw new Exception ("In plan " + GetType ().Name + ": entry point method '"+planEntryPointMethod.Name+"' must include only one parameter of type Dictionary<string,object>.");
			if (!planEntryPointMethod.GetParameters () [0].ParameterType.IsEquivalentTo (typeof(Dictionary<string,object>)))
				throw new Exception ("In plan " + GetType ().Name + ": entry point method's parameter must be of type Dictionary<string,object>.");
		}

		/// <summary>
		/// Executes a plan step.
		/// </summary>
		/// <param name="step_name">The step name.</param>
		/// <param name="args">The step arguments.</param>
		protected void ExecuteStep(string step_name, Dictionary<string,object> args = null)
		{
			bool plan_found = false;

			foreach (PlanStep step in Steps) 
			{
				if (!step.Name.Equals (step_name))
					continue;

				plan_found = true;
				step.Execute (args);
			}

			//Throw an exception if no plan step named [plan_step] has been found
			if (!plan_found) 
				throw new Exception ("In plan " + Name + ": cannot find plan step '"+step_name+"'");
		}

		protected void ExecuteExternalPlan()
		{
			
			// TODO implementare invocazione piani esterni
		}




		protected void RegisterResult(string result)
		{
			RegisterResultEvent (result);
		}

	}
}

