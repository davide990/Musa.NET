using FormulaLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Net.Sockets;
using MusaCommon;

namespace AgentLibrary
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
            get;// { return env; }
            private set;// { env = value; }
        }
        //AgentEnvironement env;
        
		private bool HostOpened;


        public ILogger Logger
        {
            get;
            private set;
        }

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
			HostOpened = false;
        }

        /// <summary>
        /// Attachs a logger to this class.
        /// </summary>
        /// <param name="Logger">Logger.</param>
        public void AttachLogger(ILogger Logger)
        {
            this.Logger = Logger;
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
        /// <param name="port">the port used by this environment</param>
        /// <param name="local_ip_address">the IP address of the machine in 
        /// which this environment is located</param>
        public void StartNetworking(string port, string local_ip_address = "localhost")
        {
            Uri address = new Uri("http://" + local_ip_address + ":" + port);
            Host = new ServiceHost(this);
            Host.AddServiceEndpoint(typeof(IMusaCommunicationService), new BasicHttpBinding(), address);
			Host.Opened += delegate 
			{
				HostOpened = true;
			};

			try
			{
				Host.Open();
			}
			catch(SocketException ex)
			{
                Logger.Log(LogLevel.Error, "Cannot setup networking for MUSA.net.\n Error: " + ex);
                return;
			}

            Logger.Log(LogLevel.Trace, "Networking setup done. Address: " + address);

            // Once networking service is active, raise an event
            if (onNetworkServiceStart != null)
                onNetworkServiceStart.Invoke(this, null);
        }

        /// <summary>
        /// Stop the networking service for this environment
        /// </summary>
        public void StopNetworkingService()
        {
			if (!HostOpened)
				return;

			HostOpened = false;
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
			
			receiver.MailBox.Push (new Tuple<AgentPassport, AgentMessage> (senderData, message));
            
            return true;
        }

        public bool sendBroadcastMessage(AgentPassport senderData, EnvironementData receiverData, AgentMessage message)
        {
            if (!AgentIsAuthorized(senderData))
                return false;

            foreach (Agent a in Environment.RegisteredAgents)
            {
				a.MailBox.Push (new Tuple<AgentPassport, AgentMessage> (senderData, message));
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

            foreach (Formula f in Environment.RegisteredAgents.FirstOrDefault(s => s.Name.Equals(agent.AgentName)).Beliefs)
                outList.Add(f.ToString());

            return outList;
        }

        public List<string> GetAgentAssignments(AgentPassport agent)
        {
            List<string> outList = new List<string>();

            foreach (AssignmentType f in Environment.RegisteredAgents.FirstOrDefault(s => s.Name.Equals(agent.AgentName)).Assignments)
                outList.Add(f.ToString());

            return outList;
        }

        public bool QueryAgent(AgentPassport sender, AgentPassport receiver, string formula)
        {
            if (!AgentIsAuthorized(sender))
                return false;

            Formula ff = FormulaParser.Parse(formula);
            return Environment.RegisteredAgents.FirstOrDefault(s => s.Name.Equals(receiver.AgentName)).TestCondition(ff);
        }

        public bool RegisterAgent(AgentPassport newAgent)
        {
            if (!AgentIsAuthorized(newAgent))
                return false;
            
            Agent a = new Agent(newAgent.AgentName);

            //TODO set workgroup, role and other attributes here

            AgentEnvironement.GetInstance().RegisterAgent(a);
            return true;
        }

		/// <summary>
		/// Gets the informations of an agent.
		/// </summary>
		/// <returns>The agentinfo.</returns>
		/// <param name="agent_name">Agent name.</param>
		public AgentPassport GetAgentinfo (string agent_name)
    	{
			Agent ag = Environment.RegisteredAgents.FirstOrDefault (s => s.Name.Equals (agent_name));

			AgentPassport ag_passport 	= new AgentPassport ();
			ag_passport.AgentName 		= ag.Name;
			ag_passport.AgentRole 		= ag.Role;
			ag_passport.CreatedAt 		= ag.CreatedAt.ToString(@"hh\:mm\:ss");
			//TODO set other agent informations here

			return ag_passport;
    	}

		public bool AddStatement (string agent_name, string statement)
    	{
			Agent ag = Environment.RegisteredAgents.FirstOrDefault (s => s.Name.Equals (agent_name));
            ag.AddBelief(new Formula[] { FormulaParser.Parse(statement) });
			//ag.Workbench.AddStatement (FormulaParser.Parse (statement));
			return true;
    	}

        #endregion
    }
}
