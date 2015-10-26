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
		/// The Quartz.net's IJobDetail related to this plan instance
		/// </summary>
		internal IJobDetail Job
		{
			get { return job; }
		}
		private IJobDetail job;

		/// <summary>
		/// The Quartz.net's ITrigger related to this plan instance
		/// </summary>
		internal ITrigger Trigger
		{
			get { return trigger; }
		}
		private ITrigger trigger;

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

		#endregion Fields/Properties

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="PlanInstance"/>class.
		/// </summary>
		public PlanInstance ()
		{
			//Generate a unique key for this plan instance
			PlanKey = new JobKey (typeof(T).Name).ToString();

			//Parse the trigger condition of the plan model
			string trigger_condition = (Activator.CreateInstance (typeof(T)) as PlanModel).TriggerCondition;
			TriggerCondition = FormulaParser.Parse (trigger_condition);

			//Use reflection to create an instance of IJobDetail for a generic PlanModel
			var create_method 					= typeof(JobBuilder).GetMethods().Where(x => x.Name == "Create").First(x => x.ContainsGenericParameters);
			MethodInfo Job_Create_method 		= create_method.MakeGenericMethod(typeof(T));
			MethodInfo Job_withIdentity_method 	= typeof(JobBuilder).GetMethod ("WithIdentity", new Type[] { typeof(string) });
			MethodInfo Job_Build_method 		= typeof(JobBuilder).GetMethod ("Build");

			JobBuilder builder = Job_Create_method.Invoke(null, null) as JobBuilder;
			builder = Job_withIdentity_method.Invoke (builder, new object[]{ typeof(T).Name }) as JobBuilder;
			job 	= Job_Build_method.Invoke (builder, null) as IJobDetail;

			//Create a new trigger for the plan
			trigger = TriggerBuilder.Create()
				.WithIdentity(typeof(T).Name + "_trigger", "trigger_group")
				.StartNow()       
				.Build();
		}

		#endregion Constructor

		#region Methods

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
				builder = builder.WithSchedule (CronScheduleBuilder.AtHourAndMinuteOnGivenDaysOfWeek (schedule.Hour, schedule.Minute, schedule.DaysOfWeek));
				break;

			case CronScheduleType.DailyAtHourAndMinute:
				builder = builder.WithSchedule (CronScheduleBuilder.DailyAtHourAndMinute (schedule.Hour, schedule.Minute));
				break;

			case CronScheduleType.MonthlyOnDayAndHourAndMinute:
				builder = builder.WithSchedule (CronScheduleBuilder.MonthlyOnDayAndHourAndMinute (schedule.DayOfMonth, schedule.Hour, schedule.Minute));
				break;

			case CronScheduleType.WeeklyOnDayAndHourAndMinute:
				builder = builder.WithSchedule (CronScheduleBuilder.WeeklyOnDayAndHourAndMinute (schedule.DayOfWeek, schedule.Hour, schedule.Minute));
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

