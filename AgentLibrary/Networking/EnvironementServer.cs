using System;
using System.Linq;
using System.ServiceModel;

namespace AgentLibrary.Networking
{
    /// <summary>
    /// This object handles the communications betweeen agents and environement.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    internal class EnvironmentServer : IMusaCommunicationService
    {
        #region Fields/Properties

        /// <summary>
        /// 
        /// </summary>
        private ServiceHost Host;

        /// <summary>
        /// 
        /// </summary>
        public AgentEnvironement Environment
        {
            get { return env; }
            private set { env = value; }
        }
        AgentEnvironement env;
        
        #endregion
        
        #region Events

        /// <summary>
        /// Event triggered when this environment starts to listen for external incoming messages, 
        /// that is, when networking service is active.
        /// </summary>
        public EventHandler onNetworkServiceStart = null;

        #endregion

        public EnvironmentServer(AgentEnvironement env)
        {
            Environment = env;
        }

        /// <summary>
        /// Setup and start the networking service for this environment
        /// </summary>
        /// <param name="address">the IP address of the machine in which this environment is located</param>
        /// <param name="port">the port used by this environment</param>
        public void StartNetworking(string port, string local_ip_address = "localhost")
        {
            Uri address = new Uri("http://" + local_ip_address + ":" + port);
            Host = new ServiceHost(this);
            Host.AddServiceEndpoint(typeof(IMusaCommunicationService), new BasicHttpBinding(), address);
            Host.Open();

            // Once networking service is active, raise an event
            if (onNetworkServiceStart != null)
                onNetworkServiceStart.Invoke(this, null);
        }

        /// <summary>
        /// Stop the networking service for this environment
        /// </summary>
        public void StopNetworkingService()
        {
            Host.Close();
        }

        public bool AgentIsActive(AgentPassport senderData, EnvironementData receiverData)
        {

            

            return true;
        }

        public bool RequestAuthorizationKey(EnvironementData env)
        {
            throw new NotImplementedException();
        }

        public bool sendAgentMessage(AgentPassport senderData, AgentPassport receiverData, AgentMessage message)
        {
            //sender agent is trusted? if not return false
            //otherwise forward the message to receiver agent

            bool senderIsTrusted = true; //for now, sender is always trusted

            if (!senderIsTrusted)
                return false;

            //find the agent to which the message must be forwarded

            Agent receiver = Environment.RegisteredAgents.FirstOrDefault(x => x.Name.Equals(receiverData.AgentName));

            if (receiver == null)
                return false;

            ///lock the receiver agent mail box, and add to it the message
            lock(receiver.lock_mailBox)
            {
                
                receiver.mailBox.Add(receiverData, message);
            }
            Console.WriteLine("Added " + message.Message);

            return true;
        }

        public bool sendBroadcastMessage(AgentPassport senderData, EnvironementData receiverData, AgentMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
