using System.ServiceModel;
using System.ServiceModel.Web;

namespace AgentLibrary.Networking
{
    /// <summary>
    /// An interface that define the communication protocol between agents and environements
    /// </summary>
    [ServiceContract]
    public interface IMusaCommunicationService
    {
        /// <summary>
        /// Sends a message to an agent
        /// </summary>
        /// <returns><c>true</c>, if agent message was sent, <c>false</c> otherwise.</returns>
        /// <param name="senderData">Sender data.</param>
        /// <param name="receiverData">Receiver data.</param>
        /// <param name="message">Message.</param>
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool sendAgentMessage(AgentPassport senderData, AgentPassport receiverData, AgentMessage message);

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
        bool sendBroadcastMessage(AgentPassport senderData, EnvironementData receiverData, AgentMessage message);

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
        bool AgentIsActive(AgentPassport senderData, EnvironementData receiverData);

        /// <summary>
        /// Requests an authorization key to access a specific environment.
        /// </summary>
        /// <returns>The authorization key.</returns>
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool RequestAuthorizationKey(EnvironementData env);


    }
}
