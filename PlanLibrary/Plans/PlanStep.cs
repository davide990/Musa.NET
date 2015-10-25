using System;
using System.Collections.Generic;

namespace PlanLibrary
{
	public class PlanStep
	{
		/// <summary>
		/// Gets the parent plan.
		/// </summary>
		/// <value>The parent.</value>
		public PlanModel Parent
		{
			get { return parent; }
			private set { parent = value; }
		}
		private PlanModel parent;


		/// <summary>
		/// Gets the trigger condition.
		/// </summary>
		/// <value>The trigger condition.</value>
		public string TriggerCondition
		{
			get { return trigger_condition; }
			private set { trigger_condition = value; }
		}
		private string trigger_condition;

		/// <summary>
		/// Gets the name of this plan step.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get { return name; }
			private set { name = value; }
		}
		private string name;

		internal PlanStep (PlanModel parent, string name, string trigger_condition = "")
		{
			Parent = parent;
			Name = name;
			TriggerCondition = trigger_condition;
		}
	}
}

