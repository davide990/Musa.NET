﻿//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  EnvironementServer.cs
//
//  Author:
//       Davide Guastella <davide.guastella90@gmail.com>
//
//  Copyright (c) 2016 Davide Guastella
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

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
    internal class EnvironmentServer : IMusaServer
    {
        #region Fields/Properties

        /// <summary>
        /// 
        /// </summary>
        private ServiceHost Host;

        /// <summary>
        /// 
        /// </summary>
        /*public AgentEnvironement Environment
        {
            get;
            private set;
        }*/

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

        public EnvironmentServer()
        {
            
            HostOpened = false;

            //Inject the logger
            Logger = ModuleProvider.Get().Resolve<ILogger>();
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
            if (string.IsNullOrEmpty(local_ip_address))
                local_ip_address = "localhost";

            Uri address = new Uri("http://" + local_ip_address + ":" + port);
            Host = new ServiceHost(this);
            Host.AddServiceEndpoint(typeof(IMusaServer), new BasicHttpBinding(), address);
            Host.Opened += delegate
            {
                HostOpened = true;
            };

            try
            {
                Host.Open();
            }
            catch (SocketException ex)
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

            var receiverEnv = AgentEnvironement.GetRootEnv();
            if (receiver.EnvironementName != AgentEnvironement.RootEnvironementName)
                receiverEnv = AgentEnvironement.GetRootEnv().GetEnvironement(receiver.EnvironementName);

            return receiverEnv.RegisteredAgents.FirstOrDefault(x => x.Name.Equals(receiver.AgentName)).IsActive;
        }

        public bool RequestAuthorizationKey(EnvironementData env)
        {
            throw new NotImplementedException();
        }

        public AgentMessage sendAgentMessage(AgentPassport senderData, AgentPassport receiverData, AgentMessage message)
        {
            if (!AgentIsAuthorized(senderData))
                throw new Exception("Agent '" + senderData.AgentName + "' is not authorized.");

            var receiverEnv = AgentEnvironement.GetRootEnv();
            if (receiverData.EnvironementName != AgentEnvironement.RootEnvironementName)
                receiverEnv = AgentEnvironement.GetRootEnv().GetEnvironement(receiverData.EnvironementName);

            //find the agent to which the message must be forwarded
            Agent receiver = receiverEnv.RegisteredAgents.FirstOrDefault(x => x.Name.Equals(receiverData.AgentName));

            if (receiver == null)
                return null;

            if (message.InfoType == InformationType.AskOne)
                return processAskOneMessages(receiver, message);

            if (message.InfoType == InformationType.AskAll)
                return processAskAllMessages(receiver, message);

            receiver.AddToMailbox(senderData, message);
            return null;
        }

        private AgentMessage processAskAllMessages(Agent receiver, AgentMessage message)
        {
            AgentMessage response = new AgentMessage();

            List<IFormula> outFormula;
            List<IAssignment> outAssignments;

            var received_formula = ModuleProvider.Get().Resolve<IFormulaUtils>().Parse(message.GetInformation() as string);
            bool success = receiver.TestCondition(received_formula, out outFormula, out outAssignments);

            if (!success)
            {
                //If no agent is able to reply, return an empty message
                return response;
            }

            //TODO
            //response.Args = assignments;
            response.InfoType = InformationType.AskAll;
            response.Sender = receiver.Name;
            outFormula.ForEach(x => response.AddInfo(x));
            return response;
        }

        /// <summary>
        /// Process messages of type AskOne.
        /// </summary>
        /// <param name="receiver">The receiver agent that process the message</param>
        /// <param name="message">The message to be processed</param>
        /// <returns></returns>
        private AgentMessage processAskOneMessages(Agent receiver, AgentMessage message)
        {
            AgentMessage response = new AgentMessage();
            List<IAssignment> assignments;
            IFormula unifiedFormula;

            //Parse the received formula
            IFormula messageFormula;
            if (message.GetInformation() is IFormula)
                messageFormula = message.GetInformation() as IFormula;
            else
                messageFormula = ModuleProvider.Get().Resolve<IFormulaUtils>().Parse(message.GetInformation() as string);

            //Test the received formula within the receiver workbench
            bool success = receiver.TestCondition(messageFormula, out assignments);

            //Set the initial response informations
            response.InfoType = InformationType.AskOne;
            response.Sender = receiver.Name;

            //return an empty message if the formula is not satisfied
            if (!success)
                return response;

            unifiedFormula = messageFormula.Clone() as IFormula;
            unifiedFormula.Unify(assignments);
            //TODO
            //response.Args = assignments;
            response.AddInfo(unifiedFormula);

            //return the response to the requester agent
            return response;
        }

        public bool sendBroadcastMessage(AgentPassport senderData, MessageScope scope, AgentMessage message)
        {
            if (!AgentIsAuthorized(senderData))
                return false;
            
            /*foreach (Agent a in GetEnvironement(senderData).RegisteredAgents)
                a.AddToMailbox(senderData, message);*/

            switch (scope)
            {
                case MessageScope.AgentEnvironment:
                    doSendBroadcastMessageToAgentEnv(senderData, message);
                    break;

                case MessageScope.All:
                    doSendBroadcastMessageToAllEnv(senderData, AgentEnvironement.GetRootEnv(), message);
                    break;

                case MessageScope.Workgroup:
                    doSendBroadcastMessageToAgentWorkgroup(senderData, message);
                    break;
            }    

            return true;
        }

        private void doSendBroadcastMessageToAgentWorkgroup(AgentPassport senderData, AgentMessage message)
        {
            //TODO 
        }

        private void doSendBroadcastMessageToAgentEnv(AgentPassport senderData, AgentMessage message)
        {
            foreach (Agent a in GetEnvironement(senderData).RegisteredAgents)
                a.AddToMailbox(senderData, message);
        }

        private void doSendBroadcastMessageToAllEnv(AgentPassport senderData, AgentEnvironement env, AgentMessage message)
        {
            foreach (Agent a in env.RegisteredAgents)
                a.AddToMailbox(senderData, message);

            if (env.SubEnvironements.Count > 0)
                env.SubEnvironements.ForEach(x => doSendBroadcastMessageToAllEnv(senderData, x, message));
        }

        public List<string> GetAgentList(AgentPassport sender)
        {
            if (!AgentIsAuthorized(sender))
                return null;
            
            return GetEnvironement(sender).RegisteredAgents.Select(s => s.Name).ToList();
        }

        public List<string> GetAgentStatements(AgentPassport agent)
        {
            List<string> outList = new List<string>();

            foreach (IFormula f in GetEnvironement(agent).RegisteredAgents.FirstOrDefault(s => s.Name.Equals(agent.AgentName)).Beliefs)
                outList.Add(f.ToString());

            return outList;
        }

        public List<string> GetAgentPlans(AgentPassport agent)
        {
            List<string> the_plan_list = new List<string>();
            Agent the_agent = GetEnvironement(agent).RegisteredAgents.First(x => x.Name.Equals(agent.AgentName));

            foreach (var the_plan in the_agent.Plans)
                the_plan_list.Add(the_plan.Name);

            return the_plan_list;
        }

        public bool QueryAgent(AgentPassport sender, AgentPassport receiver, string formula)
        {
            if (!AgentIsAuthorized(sender))
                return false;

            var FormulaParser = ModuleProvider.Get().Resolve<IFormulaUtils>();
            IFormula ff = FormulaParser.Parse(formula);
            var receiver_agent = GetEnvironement(receiver).RegisteredAgents.FirstOrDefault(s => s.Name.Equals(receiver.AgentName));

            if (receiver_agent != null)
                return receiver_agent.TestCondition(ff);

            return false;
        }

        public bool RegisterAgent(AgentPassport newAgent)
        {
            if (!AgentIsAuthorized(newAgent))
                return false;

            Agent a = new Agent(newAgent.AgentName);

            //TODO set workgroup, role and other attributes here

            AgentEnvironement.GetRootEnv().RegisterAgent(a);
            return true;
        }

        /// <summary>
        /// Gets the informations of an agent.
        /// </summary>
        /// <returns>The agentinfo.</returns>
        /// <param name="agent_name">Agent name.</param>
        public AgentPassport GetAgentinfo(string agent_name)
        {
            Agent ag = AgentEnvironement.GetRootEnv().RegisteredAgents.FirstOrDefault(s => s.Name.Equals(agent_name));
            return ag.GetPassport();
        }

        //DA ELIMINARE
        public bool AddStatement(string agent_name, string statement)
        {
            var FormulaParser = ModuleProvider.Get().Resolve<IFormulaUtils>();

            Agent ag = AgentEnvironement.GetRootEnv().RegisteredAgents.FirstOrDefault(s => s.Name.Equals(agent_name));
            ag.AddBelief(new IFormula[] { FormulaParser.Parse(statement) });

            return true;
        }

        /// <summary>
        /// Gets the environement in which the specified agent is located.
        /// </summary>
        /// <returns>The environement.</returns>
        /// <param name="agent">Agent.</param>
        private AgentEnvironement GetEnvironement(AgentPassport agent)
        {
            var env = AgentEnvironement.GetRootEnv();
            if (agent.EnvironementName != AgentEnvironement.RootEnvironementName)
                env = AgentEnvironement.GetRootEnv().GetEnvironement(agent.EnvironementName);

            return env;
        }

        #endregion
    }
}
