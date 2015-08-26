using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgentLibrary
{
    public class Schedule
    {
        public event EventHandler onScheduled;

        public Schedule()
        {
            throw new System.NotImplementedException();
        }

        public void onScheduleTriggered()
        {
            throw new System.NotImplementedException();
        }
        private AgentJobHeader jobHeader;
        private Trigger trigger;
    }
}