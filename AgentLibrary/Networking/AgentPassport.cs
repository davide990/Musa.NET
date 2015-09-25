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
        private string workgroup_name;
        private string environement_name;
        private object auth_token;

        /// <summary>
        /// The name of the workgroup the sender agent belongs to
        /// </summary>
        [DataMember]
        public string WorkgroupName
        {
            get { return workgroup_name; }
            set { workgroup_name = value; }
        }

        /// <summary>
        /// The name of the environement in which the sender agent is located
        /// </summary>
        [DataMember]
        public string EnvironementName
        {
            get { return environement_name; }
            set { environement_name = value; }
        }

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
        public object AuthorizationToken
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
