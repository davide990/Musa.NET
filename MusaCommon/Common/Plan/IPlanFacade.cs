//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  IPlanFacade.cs
//
//  Author:
//       davide <>
//
//  Copyright (c) 2016 davide
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
using System.Reflection;

namespace MusaCommon
{
    public interface IPlanFacade
    {
        IPlanInstance CreatePlanInstance(Type PlanModel, object sender, MethodInfo RegisterResultDelegate, MethodInfo PlanFinishedDelegate, ILogger Logger);
        IPlanCollection CreatePlanCollection();
        Type GetPlanInstanceTypeFor(Type planModel);
        MethodInfo GetExecuteMethodForPlan(Type PlanModel);
        void SetParentAgentFor(ref IPlanInstance plan, IAgent parent);
    }
}

