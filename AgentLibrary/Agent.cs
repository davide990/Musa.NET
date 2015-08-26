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
        private int name;
        private int ip_address;
        private int port;
        private int UUID;
        private int workbench;
        private int workScheduleStart;
        private int workScheduleEnd;

        public event EventHandler onJobReceived;

        public Agent()
        {
            throw new System.NotImplementedException();
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