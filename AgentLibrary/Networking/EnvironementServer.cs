using FormulaLibrary;
using FormulaLibrary.ANTLR;
using System;
using System.Collections.Generic;
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
        /// sender agent is trusted? if not return false
        /// otherwise forward the message to receiver agent.
        /// This check is done on agent authorization key
        /// </summary>
        /// <param name="ag"></param>
        /// <returns></returns>
        private bool AgentIsAuthorized(AgentPassport ag)
        {
            //sender agent is trusted? if not return false
            //otherwise forward the message to receiver agent
            //--> Check on authorization key
            bool senderIsTrusted = true; //for now, sender is always trusted

            if (!senderIsTrusted)
                return false;

            return true;
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

        #region IMusaCommunicationService interface methods

        public bool AgentIsActive(AgentPassport sender, AgentPassport receiver)
        {
            if (!AgentIsAuthorized(sender))
                return false;

            return Environment.RegisteredAgents.FirstOrDefault(x => x.Name.Equals(receiver.AgentName)).IsActive;
        }

        public bool RequestAuthorizationKey(EnvironementData env)
        {
            throw new NotImplementedException();
        }

        public bool sendAgentMessage(AgentPassport senderData, AgentPassport receiverData, AgentMessage message)
        {
            if (!AgentIsAuthorized(senderData))
                return false;

            //find the agent to which the message must be forwarded
            Agent receiver = Environment.RegisteredAgents.FirstOrDefault(x => x.Name.Equals(receiverData.AgentName));

            if (receiver == null)
                return false;

            //lock the receiver agent mail box, and add to it the message
            lock(receiver.lock_mailBox)
            {
                receiver.mailBox.Add(senderData, message);
            }
            
            return true;
        }

        public bool sendBroadcastMessage(AgentPassport senderData, EnvironementData receiverData, AgentMessage message)
        {
            if (!AgentIsAuthorized(senderData))
                return false;

            foreach (Agent a in Environment.RegisteredAgents)
            {
                //lock the receiver agent mail box, and add to it the message
                lock (a.lock_mailBox)
                {
                    a.mailBox.Add(senderData, message);
                }
            }
            
            return true;
        }
        
        public List<string> GetAgentList(AgentPassport sender)
        {
            if (!AgentIsAuthorized(sender))
                return null;

            return Environment.RegisteredAgents.Select(s => s.Name).ToList();
        }

        public List<string> GetAgentStatements(AgentPassport agent)
        {

            List<string> outList = new List<string>();

            foreach (Formula f in Environment.RegisteredAgents.FirstOrDefault(s => s.Name.Equals(agent.AgentName)).Workbench.Statements)
                outList.Add(f.ToString());

            return outList;
        }

        public List<string> GetAgentAssignments(AgentPassport agent)
        {
            List<string> outList = new List<string>();

            foreach (AssignmentType f in Environment.RegisteredAgents.FirstOrDefault(s => s.Name.Equals(agent.AgentName)).Workbench.AssignmentSet)
                outList.Add(f.ToString());

            return outList;
        }

        public bool QueryAgent(AgentPassport sender, AgentPassport receiver, string formula)
        {
            if (!AgentIsAuthorized(sender))
                return false;

            Formula ff = FormulaParser.Parse(formula);
            return Environment.RegisteredAgents.FirstOrDefault(s => s.Name.Equals(receiver.AgentName)).Workbench.TestCondition(ff);
        }

        public bool RegisterAgent(AgentPassport newAgent)
        {
            if (!AgentIsAuthorized(newAgent))
                return false;
            
            Agent a = new Agent(newAgent.AgentName);

            //set workgroup, role and other attributes here

            AgentEnvironement.GetInstance().RegisterAgent(a);
            return true;
        }

        #endregion
    }
}
