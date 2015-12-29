using System.Runtime.Serialization;

namespace AgentLibrary
{
    [DataContract(Name = "MessageScope")]
    public enum MessageScope
    {
        /// <summary>
        /// The scope of a message is restricted to all the agents/environments in the machine hosting the sender agent
        /// </summary>
        [EnumMember]
        All,
        /// <summary>
        /// The scope of a message is restricted to a specific environment
        /// </summary>
        [EnumMember]
        Environment,
        /// <summary>
        /// The scope of a message is restricted to a specific workgroup
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
            get;// { return address_ip; }
            set;// { address_ip = value; }
        }

        [DataMember]
        public string AddressPort
        {
            get;// { return address_port; }
            set;// { address_port = value; }
        }

        [DataMember]
        public string EnvironmentName
        {
            get;// { return env_name; }
            set;// { env_name = value; }
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
            get;// { return message; }
            set;// { message = value; }
        }

        [DataMember]
        public MessageScope MessageScope
        {
            get;// { return scope; }
            set;// { scope = value; }
        }
    }
}
