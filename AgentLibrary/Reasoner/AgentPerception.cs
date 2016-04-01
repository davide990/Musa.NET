//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  AgentPerception.cs
//
//  Author:
//       Davide Guastella <davide.guastella90@gmail.com>
//
//  Copyright (c) 2015 Davide Guastella
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
using System;
using System.Collections.Generic;
namespace AgentLibrary
{
    /// <summary>
    /// A convinient enum to mark the changes that could occur within the 
    /// environment. These changes are perceived by the agents.
    /// </summary>
    public enum AgentPerceptionType
    {

        Null,                       //No changes occurred
        AddBelief,                  //A belief has been added
        RemoveBelief,               //A belief has been removed
        UpdateBelief,               //A belief is updated. Corresponds to '-+bel' in agentspeak
        UpdateBeliefValue,          //A belief's value must be updated
        SetBeliefValue,             //A belief's value must be set
        UnSetBeliefValue,           //A belief's value must be unset
        Achieve,
        Test
    }

    public class AgentPerception
    {
        public string Formula
        {
            get;
            private set;
        }

        public AgentPerceptionType PerceptionType
        {
            get;
            private set;
        }

        public Type PlanToExecute
        {
            get;
            private set;
        }

        public List<IAssignment> Args
        {
            get;
            private set;
        }

        public AgentPerception(string formula, AgentPerceptionType perceptionType, Type planToExecute, List<IAssignment> args = null)
        {
            Formula = formula;
            PerceptionType = perceptionType;
            PlanToExecute = planToExecute;

            Args = new List<IAssignment>();
            if (args != null)
                Args.AddRange(args);
        }

    }
}

