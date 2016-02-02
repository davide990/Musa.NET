//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  PlanFacade.cs
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
using System;
using MusaCommon;
using System.Reflection;


namespace PlanLibrary
{
    [Register(typeof(IPlanFacade))]
    public class PlanFacade : MusaModule, IPlanFacade
    {
        /// <summary>
        /// Creates an instance of plan for a specific model.
        /// </summary>
        /// <returns>The instance.</returns>
        /// <param name="PlanModel">Plan model.</param>
        /// <param name="sender">The (agent) class which invoked this method. It
        /// is used also to register to agent the delegate methods to handle
        /// the plan's events for handling result and ...</param>
        /// <param name="RegisterResultDelegate">The delegate method which
        /// handles the  the registration of data within the plan.</param>
        /// <param name="PlanFinishedDelegate">The delegate method invoked when
        /// the execution of the plan has terminated.</param>
        /// <param name="Logger">The logger to be used from within the plan.</param>
        public IPlanInstance CreatePlanInstance(Type PlanModel, object sender, MethodInfo RegisterResultDelegate, MethodInfo PlanFinishedDelegate, ILogger Logger)
        {
            Type planInstanceType = typeof(PlanInstance<>).MakeGenericType(PlanModel);
            var plan_instance = Activator.CreateInstance(planInstanceType);

            //Handler for RegisterResult
            EventInfo register_result_event = planInstanceType.GetEvent("RegisterResult");
            Delegate register_plan_results_delegate = Delegate.CreateDelegate(register_result_event.EventHandlerType, sender, RegisterResultDelegate);
            register_result_event.AddEventHandler(plan_instance, register_plan_results_delegate);

            //Handler for PlanFinished
            EventInfo plan_finished_event = planInstanceType.GetEvent("Finished");
            Delegate plan_finished_delegate = Delegate.CreateDelegate(plan_finished_event.EventHandlerType, sender, PlanFinishedDelegate);
            plan_finished_event.AddEventHandler(plan_instance, plan_finished_delegate);

            //Set logger
            PropertyInfo plan_logger = planInstanceType.GetProperty("Logger", BindingFlags.NonPublic | BindingFlags.Instance);
            plan_logger.SetValue(plan_instance, Logger);

            return plan_instance as IPlanInstance;
        }

        public IPlanCollection CreatePlanCollection()
        {
            return new PlanCollection();
        }

        /// <summary>
        /// Gets a plan instance generic type for the specific model.
        /// </summary>
        /// <returns>The plan instance for.</returns>
        /// <param name="planModel">Plan model.</param>
        public Type GetPlanInstanceTypeFor(Type planModel)
        {
            return typeof(PlanInstance<>).MakeGenericType(planModel);
        }

        public MethodInfo GetExecuteMethodForPlan(Type PlanModel)
        {
            return GetPlanInstanceTypeFor(PlanModel).GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance);
        }

    }
}

