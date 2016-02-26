//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  EventKey.cs
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

namespace AgentLibrary
{
    /// <summary>
    /// The event key necessary to agent for triggering events. It is composed of a 
    /// 1st order Formula and a perception.
    /// </summary>
    public class AgentEventKey
    {
        /// <summary>
        /// The formula which the event that has this key reacts to.
        /// </summary>
        /// <value>The formula.</value>
        public string Formula
        {
            get;
            private set;
        }

        /// <summary>
        /// The perception which the event that has this key reacts to.
        /// </summary>
        /// <value>The perception.</value>
        public AgentPerception Perception
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentLibrary.EventKey"/> class.
        /// </summary>
        /// <param name="Formula">The formula which the event that has this key reacts to.</param>
        /// <param name="Perception">The perception which the event that has this key reacts to.</param>
        public AgentEventKey(string Formula, AgentPerception Perception) 
        {
            this.Formula        = Formula;
            this.Perception     = Perception;
        }
        
        public override bool Equals(object obj)
        {
            if (!(obj is AgentEventKey))
                return false;

            var the_obj = obj as AgentEventKey;
            return the_obj.Formula == Formula && the_obj.Perception == Perception;
        }

        //TODO TESTAMI
        public override int GetHashCode()
        {
            return Formula.GetHashCode() + Perception.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("[EventKey: Formula={0}, Perception={1}]", Formula, Perception);
        }
        
    }
}

