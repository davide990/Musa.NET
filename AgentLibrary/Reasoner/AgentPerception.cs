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

namespace AgentLibrary
{
    /// <summary>
    /// A convinient enum to mark the changes that could occur within the 
    /// environment. These changes are perceived by the agents.
    /// </summary>
    public enum AgentPerception
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
}

