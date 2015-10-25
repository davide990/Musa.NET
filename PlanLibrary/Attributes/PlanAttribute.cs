using System;
using System.Collections.Generic;

namespace PlanLibrary
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class PlanAttribute : Attribute
	{
		#region Properties/Fields
		/// <summary>
		/// Gets the allowed role to activate this plan.
		/// </summary>
		/// <value>The allowed role.</value>
		public List<string> AllowedRoles
		{
			get { return allowed_roles; }
			private set { allowed_roles = value; }
		}
		private List<string> allowed_roles;

		/// <summary>
		/// Gets the trigger condition necessary to activate this plan.
		/// </summary>
		/// <value>The trigger condition.</value>
		public string TriggerCondition
		{
			get { return trigger_condition; }
			private set { trigger_condition = value; }
		}
		private string trigger_condition;

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

		#endregion Properties/Fields

		#region Constructors
		public PlanAttribute ()
		{
			AllowedRoles = new List<string> ();
			AllowedRoles.Add ("all");
			TriggerCondition = "";
			ExpectedResult = "";
		}

		public PlanAttribute (string expected_result)
		{
			AllowedRoles = new List<string> ();
			AllowedRoles.Add ("all");
			TriggerCondition = "";
			ExpectedResult = "";
		}

		public PlanAttribute (string expected_result, string trigger_condition)
		{
			AllowedRoles = new List<string> ();
			AllowedRoles.Add ("all");
			TriggerCondition = trigger_condition;
			ExpectedResult = expected_result;
		}

		public PlanAttribute (string expected_result, string trigger_condition, params string[] allowed_roles)
		{
			AllowedRoles = new List<string> ();
			AllowedRoles.AddRange (allowed_roles);
			TriggerCondition = trigger_condition;
			ExpectedResult = expected_result;
		}

		#endregion Constructors
	}
}

