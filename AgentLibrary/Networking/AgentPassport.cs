using System.Runtime.Serialization;

namespace AgentLibrary.Networking
{
    
    [DataContract]
    public class AgentPassport
    {
        private string agent_address_port;
        private string agent_address_ip;
        private string agent_name;
        private string agent_role;
        private string auth_token;

        /// <summary>
        /// The port through which agents communicate
        /// </summary>
        [DataMember]
        public string AgentAddressPort
        {
            get { return agent_address_port; }
            set { agent_address_port = value; }
        }

        /// <summary>
        /// The address of the agent
        /// </summary>
        [DataMember]
        public string AgentAddressIP
        {
            get { return agent_address_ip; }
            set { agent_address_ip = value; }
        }

        /// <summary>
        /// The name of the agent
        /// </summary>
        [DataMember]
        public string AgentName
        {
            get { return agent_name; }
            set { agent_name = value; }
        }

        /// <summary>
        /// The role of the agent
        /// </summary>
        [DataMember]
        public string AgentRole
        {
            get { return agent_role; }
            set { agent_role = value; }
        }

        /// <summary>
        /// An authorization token of the agent
        /// </summary>
        [DataMember]
        public string AuthorizationToken
        {
            get { return auth_token; }
            set { auth_token = value; }
        }

        public override string ToString()
        {
            return string.Format("[AgentPassport: AgentAddressPort={0}, AgentAddressIP={1}, AgentName={2}, AgentRole={3}, AuthorizationToken={4}]", AgentAddressPort, AgentAddressIP, AgentName, AgentRole, AuthorizationToken);
        }
    }
}
