using System;
using System.Collections.Generic;

namespace PlanLibrary
{
	internal class PlanCollection
	{
		private static PlanCollection instance;

		/// <summary>
		/// Gets the plans.
		/// </summary>
		public  HashSet<PlanModel> Plans
		{
			get { return plans; }
			private set { plans = value; }
		}
		private HashSet<PlanModel> plans;

		private PlanCollection ()
		{
			plans = new HashSet<PlanModel> ();
		}

		public void AddPlan(PlanModel a)
		{
			this.plans.Add (a);
		}

		public static PlanCollection getInstance()
		{
			if (instance == null)
				instance = new PlanCollection ();

			return instance;
		}

	}
}

