//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  PlanStep.cs
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
using System.Reflection;
using MusaCommon;

namespace PlanLibrary
{
    public class PlanStep
    {
        /// <summary>
        /// Gets the parent plan.
        /// </summary>
        /// <value>The parent.</value>
        public PlanModel Parent
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the trigger condition.
        /// </summary>
        /// <value>The trigger condition.</value>
        public IFormula TriggerCondition
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name of this plan step.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return planStepMethod.Name; }
        }

        private readonly MethodInfo planStepMethod;

        public delegate void onExecuteStep(PlanStep the_step,IPlanArgs args);

        /// <summary>
        /// Occurs when this plan must be executed.
        /// </summary>
        public event onExecuteStep ExecuteStep;

        #region Constructor

        internal PlanStep(PlanModel parent, MethodInfo method, IFormula trigger_condition = null)
        {
            Parent = parent;
            planStepMethod = method;
            TriggerCondition = trigger_condition;
        }

        #endregion Constructor

        /// <summary>
        /// Execute this plan step. An event is triggered, and catched by the plan instance that contains this step. 
        /// Then, the plan instance check if the plan is in pause or not, and if not, proceed executing the method
        /// InvokePlanStep(...) that execute the plan step.
        /// </summary>
        internal void Execute(IPlanArgs args = null)
        {
            if (ExecuteStep != null)
                ExecuteStep(this, args);
            else
            {
                //TODO throw an exception here?
            }

        }

        internal void InvokePlanStep(IPlanArgs args = null)
        {
            if (planStepMethod == null)
                throw new Exception("In plan " + Parent.Name + ": invalid plan step. May be the method is not decorated with the attribute [PlanStep]?");

            //If the plan step method has parameters
            if (planStepMethod.GetParameters().Length > 0)
            {
                //If passed args are not null
                if (args != null)
                {
                    //Check if the parameter is of type Dictionary<string,object>
                    if (!planStepMethod.GetParameters()[0].ParameterType.IsEquivalentTo(typeof(Dictionary<string,object>)))
                        throw new Exception("In plan step" + Name + ": plan steps supports only a maximum of 1 parameter of type Dictionary<string,object>.");
					
                    //Invoke the method
                    //the_method.Invoke (Parent, new object[]{ args });
                    InvokePlanStepMethod(new object[]{ args });
                }
                else
                {
                    //If the passed args are null, invoke the method with an empty dictionary
                    //the_method.Invoke (Parent, new object[]{ new Dictionary<string, object> () });
                    InvokePlanStepMethod(new object[]{ new object() as IPlanArgs });
                }
            }
            else
            {
                //no args are passed neither provided by plan step method. Invoke the method without parameters.
                //the_method.Invoke (Parent, null);
                InvokePlanStepMethod(null);
            }
        }

        /// <summary>
        /// Invokes this plan step.
        /// </summary>
        /// <param name="args">Arguments.</param>
        private void InvokePlanStepMethod(object[] args)
        {
            try
            {
                planStepMethod.Invoke(Parent, args);
            }
            catch (TargetInvocationException e)
            {
                Console.WriteLine("An exception has been throwed by the invoked plan step '" + Name + "'.\nMessage: " + e.InnerException.ToString());
            }
        }


    }
}

