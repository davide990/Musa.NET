using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgentLibrary
{
    public class CronTrigger : Trigger
    {
        private int endTime;
        private int expected_deadline;
        private int startTime;

        public CronTrigger(string startDate, string endDate)
        {
            throw new System.NotImplementedException();
        }

        public int is_recurrent
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        public int recurrency_date
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        public override TriggerType getType()
        {
            throw new NotImplementedException();
        }
    }
}