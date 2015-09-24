using System.Runtime.Serialization;

namespace AgentLibrary.Networking
{
    [DataContract(Name = "MessageScope")]
    public enum MessageScope
    {
        [EnumMember]
        All,
        [EnumMember]
        Environement,
        [EnumMember]
        Collaborators
    }

    [DataContract]
    public class EnvironementData
    {
        string address_ip;
        string address_port;
        string env_name;

        [DataMember]
        public string AddressIP
        {
            get { return address_ip; }
            set { address_ip = value; }
        }

        [DataMember]
        public string AddressPort
        {
            get { return address_port; }
            set { address_port = value; }
        }

        [DataMember]
        public string EnvironementName
        {
            get { return env_name; }
            set { env_name = value; }
        }

        public override string ToString()
        {
            return string.Format("[EnvironementData: AddressIP={0}, AddressPort={1}, EnvironementName={2}]", AddressIP, AddressPort, EnvironementName);
        }
    }

    [DataContract]
    public class BroadcastMessage
    {
        private MessageScope scope;
        private string message;

        [DataMember]
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        [DataMember]
        public MessageScope MessageScope
        {
            get { return scope; }
            set { scope = value; }
        }
    }
}
