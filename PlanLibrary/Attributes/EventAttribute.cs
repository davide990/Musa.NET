using System;

namespace PlanLibrary
{
    /// <summary>
    /// Event.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class EventAttribute : Attribute
    {
        #region Properties

        /// <summary>
        /// Gets the name of the event.
        /// </summary>
        /// <value>The name of the event.</value>
        public string EventName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the trigger condition.
        /// </summary>
        /// <value>The trigger condition.</value>
        public string TriggerCondition
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name of the plan that has to be invoked when this event is triggered.
        /// </summary>
        /// <value>The name of the invoked plan.</value>
        public string PlanName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the trigger.
        /// </summary>
        /// <value>The trigger.</value>
        public EventTrigger Trigger
        {
            get;
            private set;
        }

        #endregion Properties

        public EventAttribute(string name, string condition, string plan_name, EventTrigger trigger = EventTrigger.AddStatement)
        {
            this.EventName = name;
            this.TriggerCondition = condition;
            this.PlanName = plan_name;
            this.Trigger = trigger;
        }

        public override string ToString()
        {
            return string.Format("[Event name: {0}, condition: {1}, plan_name: {2}, trigger type: {3}] ", EventName, TriggerCondition, PlanName, Trigger);
        }
    }
}

