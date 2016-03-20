//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  AgentMessage.cs
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
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace AgentLibrary
{
    /// <summary>
    /// As stated on Jason .send internal action documentation: "the 
    /// illocutionary force of the message (tell, achieve, ...)"
    /// </summary>
    [DataContract]
    public enum InformationType
    {
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Tell = 0,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Untell = 1,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Achieve = 2,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        AskOne = 3,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        AskAll = 4
    }

    [DataContract]
    public class AgentMessage
    {
        /// <summary>
        /// The name of the agent which sent this message
        /// </summary>
        [DataMember]
        public string Sender { get; set; }

        /// <summary>
        /// The content of this message
        /// </summary>
        [DataMember]
        public object Message { get; set; }

        /// <summary>
        /// Gets or sets the arguments.
        /// </summary>
        /// <value>The arguments.</value>
        [DataMember]
        public IList<object> Args { get; set; }

        /// <summary>
        /// A timestamp indicating the period within which the message is to be considered valid
        /// </summary>
        [DataMember]
        public DateTime MessageValidity { get; set; }

        /// <summary>
        /// The priority level of this message
        /// </summary>
        [DataMember]
        public byte MessagePriority { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public InformationType InfoType { get; set; }

        public override string ToString()
        {
            return string.Format("[AgentMessage: Message=[{0}], MessageValidity={1}, MessagePriority={2}, InfoType={3}]", Message, MessageValidity, MessagePriority, InfoType);
        }
    }
}
