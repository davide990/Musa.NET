using System.Collections.Generic;
using NLog;
using NLog.Targets;

namespace MusaLogger
{
    class AgentLogger : MusaLogger
    {
        private List<Target> loggerTargets;
        private bool enabled;
        private bool logBeliefs;
        private bool logWorkbenchUpdates;
        private bool logEventTrigger;
        private bool logInCommunication;
        private bool logOutCommunication;

        public List<Target> Targets
        {
            get { return loggerTargets; }
            set { loggerTargets = value; }
        }

        public bool LogBeliefsUpdate
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        public bool LogEventTrigger
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        public bool LogIncomingCommunication
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        public bool LogOutgoingCommunication
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        public bool LogWorkbenchUpdates
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        public AgentLogger(string agentName)
        {
            LogManager.GetLogger(agentName);
        }

        public void AddTarget(Target t)
        {
            loggerTargets.Add(t);
        }

        public void GetLogInterval()
        {
            throw new System.NotImplementedException();
        }

        public bool Enabled()
        {
            throw new System.NotImplementedException();
        }
    }
}
