using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgentLibrary
{
    public class Agent
    {
        private AgentScheduler scheduler;
        private List<AgentRole> roles;
        private AgentWorkbench workbench;
        private List<AgentJob> jobs;

        public event EventHandler onJobReceived;

        public Agent()
        {
            throw new System.NotImplementedException();
        }

        public Guid UUID
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        public string ip_address
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

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

        public string name
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        public DateTime workScheduleEnd
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        public DateTime workScheduleStart
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        public void manageDepartment()
        {
            throw new System.NotImplementedException();
        }

        public void scheduleJobToWorker()
        {
            throw new System.NotImplementedException();
        }
    }
}