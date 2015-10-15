/**
         __  __                                     _   
        |  \/  |                                   | |  
        | \  / | _   _  ___   __ _     _ __    ___ | |_ 
        | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
        | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
        |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|

*/
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Quartz;
using Quartz.Impl;
using System.Collections;
using AgentLibrary.Networking;

namespace AgentLibrary
{
    public class Agent
    {
        #region Fields

        /// <summary>
        /// The scheduler entity of this agent
        /// </summary>
        private AgentReasoner reasoner;

        /// <summary>
        /// The roles this agent holds
        /// </summary>
        private List<AgentRole> roles;

        /// <summary>
        /// The workbench this agent use to do reasoning activities
        /// </summary>
        private AgentWorkbench workbench;

        /// <summary>
        /// The list of jobs this agent is appointed to do
        /// </summary>
        private List<AgentJob> jobs;

        /// <summary>
        /// A unique identifier for this agent
        /// </summary>
        private readonly Guid ID;

        /// <summary>
        /// The date in which this agent started its life cycle.
        /// </summary>
        private readonly DateTime creationDate;
		public TimeSpan Uptime
		{
			get{ return DateTime.Now.Subtract(creationDate); }
		}

        /// <summary>
        /// 
        /// </summary>
        internal Thread agent_thread;

        /// <summary>
        /// This queue contains the changes to the environment that this agent have to perceive. Since an agent can be busy in doing other
        /// activities such event-handling or execution of plans, the perceived environment changes are accumulated temporarily in this
        /// queue until the agent start a perception activity.
        /// </summary>
        internal Dictionary<IList, PerceptionType> perceivedEnvironementChanges;
        internal object lock_perceivedEnvironmentChanges = new object();

        /// <summary>
        /// This dictionary represents the mailbox of the agent. In this dictionary are collected all the messages that the agent may receive
        /// from other agents in the same, or different, environment. The messages are read during the agent's reasoning cycle.
        /// </summary>
        internal Dictionary<AgentPassport, AgentMessage> mailBox;
        internal object lock_mailBox = new object();


        #endregion

        #region Events

        /// <summary>
        /// Event triggered when this agent receive a new job
        /// </summary>
        public event EventHandler jobReceived;

        #endregion

        #region Properties

        /// <summary>
        /// Return the IP address of the environment in which this agent is located
        /// </summary>
        public string EnvironementIPAddress
        {
            get { return AgentEnvironement.GetInstance().IPAddress; }
        }

        /// <summary>
        /// The workbench this agent use to do reasoning activities
        /// </summary>
        public AgentWorkbench Workbench
        {
            get { return workbench; }
        }

        /// <summary>
        /// The name  of this agent
        /// </summary>
        public string Name
        {
            get { return name; }
        }
        private readonly string name;
        
        /// <summary>
        /// Check if actually is working time for this agent.
        /// </summary>
        public bool IsActive
        {
            get { DateTime now = DateTime.UtcNow; return now >= WorkScheduleStart && now < WorkScheduleEnd; }
        }

        /// <summary>
        /// The working end time of this agent
        /// </summary>
        private DateTime _workScheduleEnd;
        public DateTime WorkScheduleEnd
        {
            get { return _workScheduleEnd; }
            private set { _workScheduleEnd = value; }
        }

        /// <summary>
        /// The working start time of this agent
        /// </summary>
        private DateTime _workScheduleStart;
        public DateTime WorkScheduleStart
        {
            get { return _workScheduleStart; }
            private set { _workScheduleStart = value; }
        }

        /// <summary>
        /// Return all the messages received by this agent
        /// </summary>
        public List<AgentMessage> MailBox
        {
            get { return new List<AgentMessage>(mailBox.Values); }
        }

        /// <summary>
        /// The role of this agent
        /// </summary>
        public string Role
        {
            get { return role; }
            set { role = value; }
        }
        private string role;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new agent
        /// </summary>
        /// <param name="agent_name"></param>
        public Agent(string agent_name)
        {
            name = agent_name;
            ID = Guid.NewGuid();
            jobReceived += onJobReceived;
            creationDate = DateTime.UtcNow;

            jobs = new List<AgentJob>();
            roles = new List<AgentRole>();
            workbench = new AgentWorkbench(this);
            reasoner = new AgentReasoner(this);
            perceivedEnvironementChanges = new Dictionary<IList, PerceptionType>();
            mailBox = new Dictionary<AgentPassport, AgentMessage>();
			creationDate = DateTime.Now;
            CreateScheduler();
        }

        #endregion

        #region Destructors

        ~Agent()
        {
            //TODO notify agent retirement
            jobs.Clear();
            roles.Clear();

            //scheduler.Shutdown();
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Get the passport of this agent.
        /// </summary>
        /// <returns></returns>
        public AgentPassport GetPassport()
        {
            AgentPassport ps = new AgentPassport();
            ps.AgentName            = Name;
            ps.AgentRole            = Role;
            ps.EnvironementAddress  = EnvironementIPAddress;
            return ps;
        }

        /// <summary>
        /// Notify to this agent a change occurred within the environment this agent is located.
        /// </summary>
        /// <param name="action">The type of change occurred</param>
        /// <param name="changes">The data that involved in the environment change</param>
        internal void notifyEnvironementChanges(PerceptionType action, IList changes)
        {
            Console.WriteLine("[Agent " + name + "] received " + action.ToString() + " -> " + changes.ToString());
            perceivedEnvironementChanges.Add(changes, action);
        }

        private void CreateScheduler()
        {
            // construct a scheduler factory
            ISchedulerFactory schedFact = new StdSchedulerFactory();

            // get a scheduler
            //scheduler = schedFact.GetScheduler();
        }

        public void manageDepartment()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Schedule a job to an employee agent
        /// </summary>
        public void scheduleJobToWorker()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Schedule a job to this agent
        /// </summary>
        public void scheduleJob(ITrigger jobTrigger, IJobDetail jobDetail)
        {
            //scheduler.ScheduleJob(jobDetail, jobTrigger);
        }

        /// <summary>
        /// Schedule a job to this agent
        /// </summary>
        public void addJob(IJobDetail jobDetail)
        {
            //scheduler.AddJob(jobDetail, false);
        }

        /// <summary>
        /// Method triggered when this agent schedule a new job.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onJobReceived(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set the working schedule for this agent.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void setWorkSchedule(DateTime start, DateTime end)
        {

        }

        /// <summary>
        /// Start the agent activity
        /// </summary>
        public Agent start()
        {
            //begin the agent reasoning activity
            reasoner.startReasoning();

            //start the scheduler
            //scheduler.Start();

            return this;
        }

        private void doPerception()
        {

        }

        #endregion




    }
}