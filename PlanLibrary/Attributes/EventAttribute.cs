using System;

namespace PlanLibrary
{
	/// <summary>
	/// Event.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	public sealed class EventAttribute : Attribute
	{
		#region Fields
		internal string agent_name;
		private string event_name;
		private string condition;
		private string plan_name;
		private EventTrigger trigger_type;
		#endregion Fields

		#region Properties
		/// <summary>
		/// Gets the name of the agent.
		/// </summary>
		/// <value>The name of the agent.</value>
		public string AgentName
		{
			get { return agent_name; }
		}

		/// <summary>
		/// Gets the name of the event.
		/// </summary>
		/// <value>The name of the event.</value>
		public string EventName
		{
			get { return event_name; }
		}

		/// <summary>
		/// Gets the trigger condition.
		/// </summary>
		/// <value>The trigger condition.</value>
		public string TriggerCondition
		{
			get { return condition; }
		}

		/// <summary>
		/// Gets the name of the linked plan.
		/// </summary>
		/// <value>The name of the linked plan.</value>
		public string LinkedPlanName
		{
			get { return plan_name; }
		}

		/// <summary>
		/// Gets the trigger.
		/// </summary>
		/// <value>The trigger.</value>
		public EventTrigger Trigger
		{
			get { return trigger_type; }
		}
		#endregion Properties

		public EventAttribute (string name, string condition, string plan_name, EventTrigger trigger = EventTrigger.AddStatement)
		{
			this.event_name = name;
			this.condition = condition;
			this.plan_name = plan_name;
			this.trigger_type = trigger;
		}

		public override string ToString ()
		{
			return string.Format ("[Event name: {0}, condition: {1}, plan_name: {2}, trigger type: {3}, agent_name: {4}] ",event_name,condition,plan_name,trigger_type, agent_name==null?"":agent_name);
		}
	}
}

