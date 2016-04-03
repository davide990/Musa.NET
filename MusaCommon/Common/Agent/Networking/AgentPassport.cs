using System.Runtime.Serialization;

namespace MusaCommon
{
    /// <summary>
    /// It is used for verifying agents identities when a communication between agents occurs.
    /// </summary>
    [DataContract]
    public class AgentPassport
    {        
        /// <summary>
        /// The name of the workgroup the agent belongs to
        /// </summary>
        [DataMember]
        public string WorkgroupName { get; set; }

        /// <summary>
        /// The name of the environment in which the agent is located
        /// </summary>
        [DataMember]
        public string EnvironementName { get; set; }

        /// <summary>
        /// The IP address of the environment in which the agent is located
        /// </summary>
        [DataMember]
        public string EnvironementAddress { get; set; }

        /// <summary>
        /// The name of the agent
        /// </summary>
        [DataMember]
        public string AgentName { get; set; }

        /// <summary>
        /// The role of the agent
        /// </summary>
        [DataMember]
        public string AgentRole { get; set; }

		/// <summary>
		/// An authorization token of the agent
		/// </summary>
		[DataMember]
        public object AuthorizationToken { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[DataMember]
        public string CreatedAt { get; set; }

        public override string ToString()
        {
            return string.Format("[AgentPassport: WorkgroupName={0}, EnvironementName={1}, EnvironementAddress={2}, AgentName={3}, AgentRole={4}, AuthorizationToken={5}, CreatedAt={6}]", WorkgroupName, EnvironementName, EnvironementAddress, AgentName, AgentRole, AuthorizationToken, CreatedAt);
        }
    }
}
