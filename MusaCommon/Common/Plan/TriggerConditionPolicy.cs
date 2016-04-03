using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusaCommon.Common.Plan
{
    enum TriggerConditionPolicy
    {
        /// <summary>
        /// The default policy: if the trigger condition is not satisfied, report an error
        /// </summary>
        Default,
        /// <summary>
        /// If a plan's trigger condition is not satisfied, do not execute the plan and do not
        /// report error.
        /// </summary>
        IgnoreError
    }
}
