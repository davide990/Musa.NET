using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MusaCommon.Common.Agent.Networking
{
    [DataContract]
    public class AgentMessageArg
    {
        [DataMember]
        public string Key
        {
            get;
            set;
        }

        [DataMember]
        public object Value
        {
            get;
            set;
        }

        public AgentMessageArg(string key, object value)
        {
            Value = value;
            Key = key;
        }

    }
}
