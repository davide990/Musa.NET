using System;
using MusaCommon;

namespace PlanLibrary
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public sealed class PlanStepAttribute : Attribute
	{
        public string TriggerCondition
        {
            get;
            private set;
		}

		public PlanStepAttribute (string trigger_condition = "")
		{
            TriggerCondition = trigger_condition;
		}
	}
}

