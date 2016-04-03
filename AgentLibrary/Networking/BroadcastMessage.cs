using System.Runtime.Serialization;

namespace AgentLibrary
{
    [DataContract(Name = "MessageScope")]
    public enum MessageScope
    {
        /// <summary>
        /// The scope of a message has no restriction, and it is sent to all the environments in the machine hosting the sender agent
        /// </summary>
        [EnumMember]
        All,
        /// <summary>
        /// The scope of a message is restricted to the specific environment in which the communicating agent is located
        /// </summary>
        [EnumMember]
        AgentEnvironment,
        /// <summary>
        /// The scope of a message is restricted to the same workgroup of the communicating agent
        /// </summary>
        [EnumMember]
        Workgroup
    }

    [DataContract]
    public class EnvironementData
    {
        /*string address_ip;
        string address_port;
        string env_name;*/

        [DataMember]
        public string AddressIP
        {
            get;
            set;
        }

        [DataMember]
        public string AddressPort
        {
            get;
            set;
        }

        [DataMember]
        public string EnvironmentName
        {
            get;
            set;
        }

        public override string ToString()
        {
            return string.Format("[EnvironementData: AddressIP={0}, AddressPort={1}, EnvironmentName={2}]", AddressIP, AddressPort, EnvironmentName);
        }
    }

    [DataContract]
    public class BroadcastMessage
    {
        /*private MessageScope scope;
        private object message;*/

        [DataMember]
        public object Message
        {
            get;
            set;
        }

        [DataMember]
        public MessageScope MessageScope
        {
            get;
            set;
        }
    }
}
