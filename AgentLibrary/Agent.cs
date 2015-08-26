using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Quartz;
using Quartz.Impl;

namespace AgentLibrary
{
    public class Agent
    {
        
        /// <summary>
        /// The scheduler entity of this agent
        /// </summary>
        private IScheduler scheduler;
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
        /// The name  of this agent
        /// </summary>
        private readonly string name;
        /// <summary>
        /// The date in which this agent started its life cycle.
        /// </summary>
        private readonly DateTime creationDate;
        /// <summary>
        /// 
        /// </summary>
        private Thread agent_thread;
        /// <summary>
        /// Check if actually is working time for this agent.
        /// </summary>
        public bool isWorking
        {
            get { DateTime now = DateTime.UtcNow;  return now >= this.workScheduleStart && now < this.workScheduleEnd; }
        }


        /// <summary>
        /// Event triggered when this agent receive a new job
        /// </summary>
        public event EventHandler jobReceived;

        public Agent(string agent_name)
        {
            this.name   = agent_name;
            this.ID     = System.Guid.NewGuid();
            jobReceived += onJobReceived;
            creationDate = DateTime.UtcNow;

            jobs        = new List<AgentJob>();
            roles       = new List<AgentRole>();
            workbench   = new AgentWorkbench();
            //scheduler   = new AgentScheduler();

            CreateScheduler();
        }

        private void CreateScheduler()
        {
            // construct a scheduler factory
            ISchedulerFactory schedFact = new StdSchedulerFactory();

            // get a scheduler
            scheduler = schedFact.GetScheduler();
        }

        
        ~Agent()
        {
            //TODO notify agent retirement
            jobs.Clear();
            roles.Clear();
            
            scheduler.Shutdown();
        }

        /// <summary>
        /// Start the agent activity
        /// </summary>
        public void start()
        {
            //start the scheduler
            scheduler.Start();
        }

        /// <summary>
        /// Return the IP address of the machine in which this agent is located
        /// </summary>
        public string ip_address
        {
            get
            {
                //return Dns.GetHostByName(hostName).AddressList[0].ToString(); DEPRECATED
                return Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString();
            }
            
        }

        /// <summary>
        /// Return the port this agent listen to.
        /// </summary>
        public string port
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        /// <summary>
        /// The working end time of this agent
        /// </summary>
        private DateTime _workScheduleEnd;
        public DateTime workScheduleEnd
        {
            get {return _workScheduleEnd;}
            private set {_workScheduleEnd = value;}
        }

        /// <summary>
        /// The working start time of this agent
        /// </summary>
        private DateTime _workScheduleStart;
        public DateTime workScheduleStart
        {
            get { return _workScheduleStart; }
            private set { _workScheduleStart = value; }
        }

        public void manageDepartment()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Schedule a job to an employee agent
        /// </summary>
        public void scheduleJobToWorker()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Schedule a job to this agent
        /// </summary>
        public void scheduleJob(ITrigger jobTrigger, IJobDetail jobDetail)
        {
            scheduler.ScheduleJob(jobDetail, jobTrigger);
        }

        /// <summary>
        /// Schedule a job to this agent
        /// </summary>
        public void addJob(IJobDetail jobDetail)
        {
            scheduler.AddJob(jobDetail, false);
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

        
    }
}