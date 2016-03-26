//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  MusaInitializer.cs
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

using AgentLibrary;
using AgentLibrary.Attributes;
using FormulaLibrary;
using MusaCommon;
using MusaLogger;
using PlanLibrary;
using System;
using System.Linq;
using System.Reflection;

namespace MusaInitializer
{
    public static class MUSAInitializer
    {
        /// <summary>
        /// Initialize all the modules necessary for MUSA to work.
        /// </summary>
        public static void Initialize()
        {
            MusaLoggerInitializer.Initialize();
            PlanLibraryInitializer.Initialize();
            FormulaLibraryInitializer.Initialize();
            //...
            //Initialize other modules (projects) here


            DiscoverAgents();
        }

        /// <summary>
        /// Discover automatically all the classes in the entry assembly which are decorated with [Agent] attribute
        /// </summary>
        private static void DiscoverAgents()
        {
            var env = AgentEnvironement.GetInstance();

            //Get the calling assembly
            var entry_assembly = System.Reflection.Assembly.GetEntryAssembly();

            var class_list = from t in entry_assembly.GetExportedTypes()
                             let attributes = t.GetCustomAttributes(typeof(AgentAttribute), false)
                             where attributes != null && attributes.Length > 0
                             select t;

            var fp = ModuleProvider.Get().Resolve<IFormulaUtils>();
            ConstructorInfo ctor = typeof(Agent).GetConstructor(new[] { typeof(string) });

            foreach (var agent_type in class_list)
            {
                //Create a new instance of agent
                var the_agent = ctor.Invoke(new object[] { agent_type.Name }) as Agent;

                //Get the attributes [Belief] in the current agent type
                var beliefs = agent_type.GetCustomAttributes<BeliefAttribute>();

                //Add the beliefs to the agent belief base
                foreach (var belief in beliefs)
                {
                    IFormula the_belief = fp.Parse(belief.Belief);
                    the_agent.AddBelief(the_belief);
                }

                //Register the agent in the environment
                env.RegisterAgent(the_agent);
            }
        }
    }
}

