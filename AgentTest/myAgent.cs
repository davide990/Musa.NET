using AgentLibrary.Attributes;
using System;
using PlanLibrary;
using AgentLibrary;
using MusaCommon;
using System.Collections.Generic;
using FormulaLibrary;

namespace AgentTest
{
    [Agent]
    [Belief("k(p)")]
    public class myAgent : Agent
    {


        public myAgent()
        {
            AddEvent("f(3)", AgentPerception.AddBelief, typeof(myPlan));

        }

        [Plan]
        public class myPlan : PlanModel
        {
            [PlanEntryPoint]
            public void entryPoint(IPlanArgs args)
            {
                var fp = ModuleProvider.Get().Resolve<IFormulaUtils>();
                var af = ModuleProvider.Get().Resolve<IAssignmentFactory>();

                List<IAssignment> assignments;
                var the_formula = fp.Parse("f(x)");
                Parent.TestCondition(the_formula, out assignments);
                var x_value = assignments.Find(x => x.GetName().Equals("x"));
                var new_assignment = new Assignment<int>(x_value.GetName(), (int)x_value.GetValue() - 1);
                the_formula.Unify(new_assignment);
                Parent.UpdateBelief(the_formula);

                var parent_name = Parent.GetName();
                
            }
        }

    }
}
