//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  IPlanInstance.cs
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

namespace MusaCommon
{
    public interface IPlanInstance
    {
        /// <summary>
        /// Gets the name of this plan.
        /// </summary>
        string GetName();

        /// <summary>
        /// Pause this plan instance.
        /// </summary>
        void Pause();

        /// <summary>
        /// Resume this plan instance.
        /// </summary>
        void Resume();

        /// <summary>
        /// Execute this plan.
        /// </summary>
        void Execute(IPlanArgs args = null);

        /// <summary>
        /// Gets the plan's trigger condition, necessary for invoking this plan.
        /// </summary>
        /// <returns>The trigger condition.</returns>
        IFormula GetTriggerCondition();

        /// <summary>
        /// Set the agent who invoke this plan instance
        /// </summary>
        /// <param name="source"></param>
        void SetSourceAgent(IAgent source);

        /// <summary>
        /// Determines whether this plan is atomic.
        /// </summary>
        bool IsAtomic();
    }
}

