﻿using MusaCommon;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MusaCommon
{
    public interface IAgent
    {
        string GetName();
        IAgentWorkbench GetWorkbench();

        bool TestCondition(IFormula formula);
        bool TestCondition(IFormula formula, out List<IAssignment> generated_assignments);
        bool TestCondition(IFormula formula, out List<IFormula> unifiedPredicates, out List<IAssignment> generated_assignments);
        void AchieveGoal(Type Plan, PlanArgs Args = null);
        void AddBelief(IAgent source, params IFormula[] formula);
        void AddBelief(params IFormula[] formula);
        void AddBelief(IList formula_list);
        void UpdateBelief(params IFormula[] formula);
        void UpdateBelief(IList formula_list);
        void RemoveBelief(params IFormula[] formula);
        void RemoveBelief(IList formula_list);
        void SendLocalMessage(string agentReceiverName, AgentMessage message);
        void SendLocalMessage(AgentPassport receiver, AgentMessage message);
        void SendLocalBroadcastMessage(AgentMessage message);
    }
}
