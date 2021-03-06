﻿using MusaCommon;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace AgentLibrary
{
    /// <summary>
    /// An interface that define the communication protocol between agents and environements
    /// </summary>
    [ServiceContract]
    public interface IMusaServer
    {
        /// <summary>
        /// Sends a message to an agent
        /// </summary>
        /// <returns>A response message</returns>
        /// <param name="senderData">Sender data.</param>
        /// <param name="receiverData">Receiver data.</param>
        /// <param name="message">Message.</param>
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        AgentMessage sendAgentMessage(AgentPassport senderData, AgentPassport receiverData, AgentMessage message);

        /// <summary>
        /// Sends the broadcast message.
        /// </summary>
        /// <returns><c>true</c>, if broadcast message was sent, <c>false</c> otherwise.</returns>
        /// <param name="senderData">Sender data.</param>
        /// <param name="receiverData">Receiver data.</param>
        /// <param name="message">Message.</param>
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool sendBroadcastMessage(AgentPassport senderData, MessageScope scope, AgentMessage message);

        /// <summary>
        /// Check if an agent is active. E' una sorta di metodo per tuppuliare un agente
        /// </summary>
        /// <returns><c>true</c>, if is active was agented, <c>false</c> otherwise.</returns>
        /// <param name="senderData">Sender data.</param>
        /// <param name="receiverData">Receiver data.</param>/// 
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool AgentIsActive(AgentPassport senderData, AgentPassport receiverData);

        /// <summary>
        /// Requests an authorization key to access a specific environment.
        /// </summary>
        /// <returns>The authorization key.</returns>
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool RequestAuthorizationKey(EnvironementData env);

        /// <summary>
        /// Return the list of agents within the environment
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<string> GetAgentList(AgentPassport sender);
        
        /// <summary>
        /// Return the statements within the workbench of an agent
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<string> GetAgentStatements(AgentPassport agent);

        /// <summary>
        /// Return the plans the specified agent has
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<string> GetAgentPlans(AgentPassport agent);

        /// <summary>
        /// Ask to an agent to verify in its workbench a given formula
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool QueryAgent(AgentPassport sender, AgentPassport receiver, string formula);

        /// <summary>
        /// Register a new agent to the environment
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool RegisterAgent(AgentPassport newAgent);

		/// <summary>
		/// Gets the informations of an agent.
		/// </summary>
		/// <returns>The agentinfo.</returns>
		/// <param name="agent_name">Agent name.</param>
		[WebInvoke(Method = "POST",
			ResponseFormat = WebMessageFormat.Json,
			RequestFormat = WebMessageFormat.Json)]
		[OperationContract]
		AgentPassport GetAgentinfo(string agent_name);

		[WebInvoke(Method = "POST",
			ResponseFormat = WebMessageFormat.Json,
			RequestFormat = WebMessageFormat.Json)]
		[OperationContract]
		bool AddStatement(string agent_name, string statement);



    }
}
