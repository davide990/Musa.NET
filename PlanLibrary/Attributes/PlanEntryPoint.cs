using System;

namespace PlanLibrary
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public class PlanEntryPoint : Attribute
	{
		public PlanEntryPoint ()
		{}
	}
}

