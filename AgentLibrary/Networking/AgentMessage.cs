using System;
using System.Runtime.Serialization;

namespace AgentLibrary.Networking
{
    [DataContract]
    public class AgentMessage
    {
        private object message;
        private DateTime validity;
        private byte messagePriority;

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

        public override string ToString()
        {
            return string.Format("[AgentMessage: Message=[{0}],\n MessageValidity={1},\n MessagePriority={2}]", Message, MessageValidity, MessagePriority);
        }
    }
}
