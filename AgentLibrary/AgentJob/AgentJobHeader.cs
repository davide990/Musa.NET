using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgentLibrary
{
    public class AgentJobHeader
    {
        private int job_description;
        private int post_condition;

        public AgentJob AgentJob
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        public void getJob()
        {
            throw new System.NotImplementedException();
        }

        public AgentJobHeader(AgentJob job)
        {
            this.AgentJob = job;
        }
    }
}