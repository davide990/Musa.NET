using System;

namespace PlanLibrary
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class AtomicPlanAttribute : Attribute
	{
		public AtomicPlanAttribute ()
		{
		}
	}
}

