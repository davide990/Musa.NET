using System;
using System.Collections.Generic;
using System.Reflection;

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
			get { return the_method.Name; }
		}
		private MethodInfo the_method;

		internal PlanStep (PlanModel parent, MethodInfo method, string trigger_condition = "")
		{
			Parent = parent;
			the_method = method;
			TriggerCondition = trigger_condition;
		}

		internal void Execute(/*object[] args = null*/)
		{
			if (the_method == null)
				throw new Exception ("In plan " + Parent.Name + ": invalid plan step.");

			the_method.Invoke (this, null);
		}
	}
}

