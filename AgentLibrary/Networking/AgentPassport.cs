using System.Runtime.Serialization;

namespace AgentLibrary
{
    /// <summary>
    /// It is used for verifying agents identities when a communication between agents occurs.
    /// </summary>
    [DataContract]
    public class AgentPassport
    {
        /*private string agent_name;
        private string agent_role;
        private string workgroup_name;
        private string environement_name;
        private string environement_ipaddress;
		private string createdAt;
        private object auth_token;*/
        
        /// <summary>
        /// The name of the workgroup the agent belongs to
        /// </summary>
        [DataMember]
        public string WorkgroupName
        {
            get;// { return workgroup_name; }
            set;// { workgroup_name = value; }
        }

        /// <summary>
        /// The name of the environment in which the agent is located
        /// </summary>
        [DataMember]
        public string EnvironementName
        {
            get;// { return environement_name; }
            set;// { environement_name = value; }
        }

        /// <summary>
        /// The IP address of the environment in which the agent is located
        /// </summary>
        [DataMember]
        public string EnvironementAddress
        {
            get;// { return environement_ipaddress; }
            set;// { environement_ipaddress = value; }
        }

        /// <summary>
        /// The name of the agent
        /// </summary>
        [DataMember]
        public string AgentName
        {
            get;// { return agent_name; }
            set;// { agent_name = value; }
        }

        /// <summary>
        /// The role of the agent
        /// </summary>
        [DataMember]
        public string AgentRole
        {
            get;// { return agent_role; }
            set;// { agent_role = value; }
        }

		/// <summary>
		/// An authorization token of the agent
		/// </summary>
		[DataMember]
		public object AuthorizationToken
		{
            get;// { return auth_token; }
            set;// { auth_token = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		[DataMember]
		public string CreatedAt
		{
            get;// { return createdAt; }
            set;// { createdAt = value; }
		}

        public override string ToString()
        {
            return string.Format("[AgentPassport: AgentName={0}, AgentRole={1}, AuthorizationToken={2}]", AgentName, AgentRole, AuthorizationToken);
        }
    }
}
