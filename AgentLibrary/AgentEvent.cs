//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  AgentEvent.cs
//
//  Author:
//       Davide Guastella <davide.guastella90@gmail.com>
//
//  Copyright (c) 2015 Davide Guastella
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
using MusaCommon;

namespace AgentLibrary
{
    /// <summary>
    /// An event that an agent could reacts to.
    /// </summary>
    public class AgentEvent
    {
        /// <summary>
        /// Gets the formula this event is triggered by.
        /// </summary>
        /// <value>The formula.</value>
        public string Formula
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the type of perception this event reacts to.
        /// </summary>
        /// <value>The perception.</value>
        public AgentPerception Perception
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the event key.
        /// </summary>
        /// <value>The event key.</value>
        public AgentEventKey EventKey
        {
            get { return new AgentEventKey(Formula, Perception); }
        }

        /// <summary>
        /// Gets the plan that is invoked when this event is triggered
        /// </summary>
        /// <value>The plan.</value>
        public Type Plan
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the arguments for this event. These are passed to the invoked
        /// plan.
        /// </summary>
        /// <value>The arguments.</value>
        public AgentEventArgs Args
        {
            get;
            private set;
        }

        public AgentEvent(AgentEventKey key, Type plan, AgentEventArgs args = null)
        {
            Formula = key.Formula;
            Perception = key.Perception;
            Plan = plan;
            Args = args;
        }

        public AgentEvent(string formula, AgentPerception perception, Type plan,
                          AgentEventArgs args = null)
        {
            Formula = formula;
            Perception = perception;
            Plan = plan;
            Args = args;
        }

    }
}

