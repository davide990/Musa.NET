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
using System.Text;
using MusaCommon;
using MusaCommon.Common.Agent.Networking;

namespace MusaCommon
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
        public List<object> Message { get; set; }

        /// <summary>
        /// Gets or sets the arguments.
        /// </summary>
        /// <value>The arguments.</value>
        [DataMember]
        public IList<AgentMessageArg> Args { get; set; }

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

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string ReplyTo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string ReplyBy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Ontology { get; set; }

        /// <summary>
        /// The number of informations contained in this message
        /// </summary>
        public int InformationsCount
        {
            get
            {
                return Message.Count;
            }
        }

        public AgentMessage(object Info = null)
        {
            Message = new List<object>();
            Args = new List<AgentMessageArg>();

            if (!string.IsNullOrEmpty(Info as string))
                Message.Add(Info);
        }

        /// <summary>
        /// Return a clone of this message. By default, the cloned message
        /// doesn't contains the information.
        /// </summary>
        /// <returns></returns>
        public object Clone(bool cloneInfo = false)
        {
            AgentMessage cloned = new AgentMessage();
            cloned.Args = Args;
            cloned.InfoType = InfoType;
            cloned.MessagePriority = MessagePriority;
            cloned.MessageValidity = MessageValidity;
            cloned.Sender = Sender;

            if (cloneInfo)
                cloned.Message = Message;

            return cloned;
        }

        /// <summary>
        /// Return the information contained in this message.
        /// </summary>
        /// <returns>The list of informations if informations count is more than 1, otherwise
        /// return the unique information contained in this message</returns>
        public object GetInformation()
        {
            if (Message.Count == 0)
                return null;
            if (Message.Count == 1)
                return Message[0];

            return Message;
        }

        /// <summary>
        /// Add an information to this message
        /// </summary>
        /// <param name="info">The information to be added. The type of object must be string or IFormula</param>
        public void AddInfo(params object[] info)
        {
            foreach (object o in info)
            {
                if (!(o is string | o is IFormula))
                    throw new Exception("Provided object is neither of type string or IFormula.");

                if (Message.Contains(o))
                    continue;

                Message.Add(o);
            }
        }

        public override string ToString()
        {
            string the_information;
            if (InformationsCount > 1)
                the_information = string.Join(",", GetInformation());
            else
                the_information = Convert.ToString(GetInformation());

            return string.Format("[AgentMessage: Message=[{0}], MessageValidity={1}, MessagePriority={2}, InfoType={3}]", the_information, MessageValidity, MessagePriority, InfoType);
        }
    }
}
