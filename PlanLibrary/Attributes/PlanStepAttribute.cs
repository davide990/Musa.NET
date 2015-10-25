using System;
using System.Reflection;
using System.Diagnostics;

namespace PlanLibrary
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public sealed class PlanStepAttribute : Attribute
	{
		public string TriggerCondition
		{
			get { return trigger_condition; }
			private set { trigger_condition = value; }
		}
		private string trigger_condition;

		public PlanStepAttribute (string trigger_condition = "")
		{
			TriggerCondition = trigger_condition;
		}
	}
}

