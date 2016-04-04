//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  IMusaClient.cs
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
using MusaCommon;
using System.Collections.Generic;

namespace AgentLibrary
{
    public interface IMusaClient
    {
        AgentMessage sendAgentMessage(AgentPassport receiverData, AgentMessage message);

        bool sendBroadcastMessage(MessageScope scope, AgentMessage message);

        bool AgentIsActive(AgentPassport receiverData);

        bool RequestAuthorizationKey(EnvironementData env);

        List<string> GetAgentList(AgentPassport agent);

        List<string> GetAgentStatements(AgentPassport agent);

        List<string> GetAgentPlans(AgentPassport agent);

        bool QueryAgent(AgentPassport receiver, string formula);

        bool RegisterAgent(AgentPassport newAgent);

        AgentPassport GetAgentinfo(string agent_name);
    }
}

