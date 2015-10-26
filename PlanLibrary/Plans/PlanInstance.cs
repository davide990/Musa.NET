using System;
using Quartz;
using System.Reflection;
using System.Linq;
using FormulaLibrary;
using FormulaLibrary.ANTLR;

namespace PlanLibrary
{
	public interface IPlanInstance
	{}

	/// <summary>
	/// Plan instance.
	/// </summary>
	public class PlanInstance<T> : IPlanInstance where T : PlanModel
	{
		#region Fields/Properties

		/// <summary>
		/// The unique key for this plan instance
		/// </summary>
		public string PlanKey
		{
			get { return plan_key; }
			private set { plan_key = value; }
		}
		private string plan_key;

		/// <summary>
		/// The trigger condition necessary to activate this plan.
		/// </summary>
		public Formula TriggerCondition
		{
			get { return triggerCondition; }		//Type of TriggerCondition here must be Formula
			private set { triggerCondition = value; }
		}
		private Formula triggerCondition;

		/// <summary>
		/// The plan model this instance references to.
		/// </summary>
		private PlanModel plan_model;

		#endregion Fields/Properties

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="PlanInstance"/>class.
		/// </summary>
		public PlanInstance ()
		{
			//Generate a unique key for this plan instance
			PlanKey = new JobKey (typeof(T).Name).ToString();

			plan_model = Activator.CreateInstance (typeof(T)) as PlanModel;

			//Parse the trigger condition of the plan model
			string trigger_condition = plan_model.TriggerCondition;

			//TriggerCondition = FormulaParser.Parse (trigger_condition);


		}

		#endregion Constructor

		#region Methods

		/// <summary>
		/// Execute this plan.
		/// </summary>
		public void Execute()
		{
			plan_model.Execute (plan_model.Args);
		}

		public void SetArgs(object[] args)
		{
			plan_model.Args = args;
		}


		/// <summary>
		/// Executes this plan
		/// </summary>
		public void SetStartNow()
		{
			/*if (!schedule_set)
				throw new Exception ("Schedule for this plan has not been set.");*/
		}


		//TODO
		/*
		public void SetSchedule(Schedule schedule)
		{
			if (schedule is CronSchedule)
				setCronSchedule (schedule as CronSchedule);
			else if (schedule is SimpleSchedule)
				setSimpleSchedule (schedule as SimpleSchedule);

		}

		//TODO
		/// <summary>
		/// Set the cron schedule for this plan
		/// </summary>
		/// <param name="schedule">Schedule.</param>
		private void setCronSchedule(CronSchedule schedule)
		{
			switch(schedule.ScheduleType)
			{
			case CronScheduleType.AtHourAndMinuteOnGivenDaysOfWeek:
				break;

			case CronScheduleType.DailyAtHourAndMinute:
				break;

			case CronScheduleType.MonthlyOnDayAndHourAndMinute:
				break;

			case CronScheduleType.WeeklyOnDayAndHourAndMinute:
				break;
			}
		}

		private void setSimpleSchedule(SimpleSchedule schedule)
		{
			//...

		}
		*/


		public void SchedulePlan()
		{
			//...
		}
		#endregion Methods
	}
}

