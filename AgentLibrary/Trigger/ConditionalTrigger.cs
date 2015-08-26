using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgentLibrary
{
    public class ConditionalTrigger : Trigger
    {
        public ConditionalTrigger(string pre_condition)
        {
            throw new System.NotImplementedException();
        }

        public int pre_condition
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