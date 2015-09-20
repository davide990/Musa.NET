﻿/**
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

namespace AgentLibrary
{
    public class Agent
    {
        
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
        public AgentWorkbench Workbench
        {
            get { return workbench; }
        }
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
        public string Name
        {
            get { return name; }
        }
        private readonly string name;
        /// <summary>
        /// The date in which this agent started its life cycle.
        /// </summary>
        private readonly DateTime creationDate;
        /// <summary>
        /// 
        /// </summary>
        internal Thread agent_thread;
        /// <summary>
        /// Check if actually is working time for this agent.
        /// </summary>
        public bool isWorkingTime
        {
            get { DateTime now = DateTime.UtcNow;  return now >= workScheduleStart && now < workScheduleEnd; }
        }

        /// <summary>
        /// This queue contains the changes to the environement that this agent have to perceive. Since an agent can be busy in doing other
        /// activities such event-handling or execution of plans, the perceived environement changes are accumulated temporarily in this
        /// queue until the agent start a perception activity.
        /// </summary>
        internal Dictionary<IList, PerceptionType> perceivedEnvironementChanges;
        internal object lock_perceivedEnvironementChanges = null;



        /// <summary>
        /// Event triggered when this agent receive a new job
        /// </summary>
        public event EventHandler jobReceived;
        
        /// <summary>
        /// Create a new agent
        /// </summary>
        /// <param name="agent_name"></param>
        public Agent(string agent_name)
        {
            name   = agent_name;
            ID     = Guid.NewGuid();
            jobReceived += onJobReceived;
            creationDate = DateTime.UtcNow;

            jobs        = new List<AgentJob>();
            roles       = new List<AgentRole>();
            workbench   = new AgentWorkbench(this);
            reasoner    = new AgentReasoner(this);
            perceivedEnvironementChanges = new Dictionary<IList, PerceptionType>();
            
            CreateScheduler();
        }
        
        /// <summary>
        /// Notify to this agent a change occurred within the environement this agent is located.
        /// </summary>
        /// <param name="action">The type of change occurred</param>
        /// <param name="changes">The data that involved in the environement change</param>
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

        
        ~Agent()
        {
            //TODO notify agent retirement
            jobs.Clear();
            roles.Clear();
            
            //scheduler.Shutdown();
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
                throw new NotImplementedException();
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
        public void start()
        {
            //begin the agent reasoning activity
            reasoner.startReasoning();

            //start the scheduler
            //scheduler.Start();
        }

        private void doPerception()
        {

        }

        
    }
}