using System;
using System.Runtime.Serialization;

namespace AgentLibrary
{
    /// <summary>
    /// As stated on Jason .send internal action documentation: "the illocutionary force of the message (tell, achieve, ...)"
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
        private object message;
        private DateTime validity;
        private byte messagePriority;
        private InformationType infoType;
        
        /// <summary>
        /// The content of this message
        /// </summary>
        [DataMember]
        public object Message
        {
            get { return message; }
            set { message = value; }
        }

        /// <summary>
        /// A timestamp indicating the period within which the message is to be considered valid
        /// </summary>
        [DataMember]
        public DateTime MessageValidity
        {
            get { return validity; }
            set { validity = value; }
        }

        /// <summary>
        /// The priority level of this message
        /// </summary>
        [DataMember]
        public byte MessagePriority
        {
            get { return messagePriority; }
            set { messagePriority = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public InformationType InfoType
        {
            get { return infoType; }
            set { infoType = value; }
        }

        public override string ToString()
        {
            return string.Format("[AgentMessage: Message=[{0}], MessageValidity={1}, MessagePriority={2}, InfoType={3}]", Message, MessageValidity, MessagePriority, InfoType);
        }
    }
}
