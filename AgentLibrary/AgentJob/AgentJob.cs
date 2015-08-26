using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;

namespace AgentLibrary
{
    /// <summary>
    /// Si identifica con il concetto di capability
    /// </summary>
    public abstract class AgentJob : IJob
    {
        private List<AgentRole> allowed_roles;
        
        public AgentJob(string name)
        {
            this._job_name = name;
        }

        private AgentJobResult _job_result;
        public AgentJobResult job_result
        {
            get { return _job_result; }
        }

        private string _job_name;
        public string job_name
        {
            get { return _job_name; }
            protected set { _job_name = value; }
        }

        public abstract void Execute(IJobExecutionContext context);
    }
}