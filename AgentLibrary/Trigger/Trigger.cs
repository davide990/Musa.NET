﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;

namespace AgentLibrary
{
    public abstract class Trigger
    {
        public abstract TriggerType getType();
    }
}