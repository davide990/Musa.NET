using System;
using System.Collections.Generic;
using System.Reflection;

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
			get { return parent; }
			private set { parent = value; }
		}
		private PlanModel parent;

		/// <summary>
		/// Gets the trigger condition.
		/// </summary>
		/// <value>The trigger condition.</value>
		public string TriggerCondition
		{
			get { return trigger_condition; }
			private set { trigger_condition = value; }
		}
		private string trigger_condition;

		/// <summary>
		/// Gets the name of this plan step.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get { return the_method.Name; }
		}
		private MethodInfo the_method;

		public delegate void onExecuteStep (PlanStep the_step, Dictionary<string, object> args);
		/// <summary>
		/// Occurs when this plan must be executed.
		/// </summary>
		public event onExecuteStep ExecuteStep;


		internal PlanStep (PlanModel parent, MethodInfo method, string trigger_condition = "")
		{
			Parent = parent;
			the_method = method;
			TriggerCondition = trigger_condition;
		}

		/// <summary>
		/// Execute this plan step. An event is triggered, and catched by the plan instance that contains this step. 
		/// Then, the plan instance check if the plan is in pause or not, and if not, proceed executing the method
		/// InvokePlanStep(...) that execute the plan step.
		/// </summary>
		internal void Execute(Dictionary<string, object> args = null)
		{
			if (ExecuteStep != null)
				ExecuteStep (this, args);
			else
			{
				//TODO throw an exception here?
			}

		}

		internal void InvokePlanStep(Dictionary<string, object> args = null)
		{
			if (the_method == null)
				throw new Exception ("In plan " + Parent.Name + ": invalid plan step. May be the method is not decorated with the attribute [PlanStep]?");

			//If the plan step method has parameters
			if (the_method.GetParameters ().Length > 0) 
			{
				//If passed args are not null
				if (args != null) 
				{
					//Check if the parameter is of type Dictionary<string,object>
					if (!the_method.GetParameters () [0].ParameterType.IsEquivalentTo (typeof(Dictionary<string,object>)))
						throw new Exception ("In plan step" + Name + ": plan steps supports only a maximum of 1 parameter of type Dictionary<string,object>.");
					
					//Invoke the method
					//the_method.Invoke (Parent, new object[]{ args });
					InvokePlanStepMethod (new object[]{ args });
				} 
				else 
				{
					//If the passed args are null, invoke the method with an empty dictionary
					//the_method.Invoke (Parent, new object[]{ new Dictionary<string, object> () });
					InvokePlanStepMethod (new object[]{ new Dictionary<string, object> () });
				}
			} 
			else 
			{
				//no args are passed neither provided by plan step method. Invoke the method without parameters.
				//the_method.Invoke (Parent, null);
				InvokePlanStepMethod (null);
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
				the_method.Invoke (Parent, args);
			}
			catch(TargetInvocationException e)
			{
				Console.WriteLine ("An exception has been throwed by the invoked plan step '"+Name+"'.\nMessage: "+e.InnerException.ToString());
			}
		}


	}
}

