//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  EventAttribute.cs
//
//  Author:
//       Davide Guastella <davide.guastella90@gmail.com>
//
//  Copyright (c) 2016 Davide Guastella
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

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

