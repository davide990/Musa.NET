//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  PlanAttribute.cs
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

namespace PlanLibrary
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class PlanAttribute : Attribute
    {
        #region Properties/Fields

        /// <summary>
        /// Gets the allowed role to activate this plan.
        /// </summary>
        /// <value>The allowed role.</value>
        public List<string> AllowedRoles
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the trigger condition necessary to activate this plan.
        /// </summary>
        /// <value>The trigger condition.</value>
        public string TriggerCondition
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the expected result.
        /// </summary>
        /// <value>The expected result.</value>
        public string ExpectedResult
        {
            get;
            private set;
        }

        #endregion Properties/Fields

        #region Constructors

        public PlanAttribute()
        {
            AllowedRoles = new List<string>();
            AllowedRoles.Add("all");
            TriggerCondition = "";
            ExpectedResult = "";
        }

        public PlanAttribute(string trigger_condition)
        {
            AllowedRoles = new List<string>();
            AllowedRoles.Add("all");
            TriggerCondition = trigger_condition;
            ExpectedResult = "";
        }

        public PlanAttribute(string trigger_condition, string expected_result)
        {
            AllowedRoles = new List<string>();
            AllowedRoles.Add("all");
            TriggerCondition = trigger_condition;
            ExpectedResult = expected_result;
        }

        public PlanAttribute(string trigger_condition, string expected_result, params string[] allowed_roles)
        {
            AllowedRoles = new List<string>();
            AllowedRoles.AddRange(allowed_roles);
            TriggerCondition = trigger_condition;
            ExpectedResult = expected_result;
        }

        #endregion Constructors
    }
}

