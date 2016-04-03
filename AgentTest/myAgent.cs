using AgentLibrary.Attributes;
using System;
using PlanLibrary;
using AgentLibrary;
using MusaCommon;
using System.Collections.Generic;
using FormulaLibrary;
using System.Threading;
using PlanLibrary.Attributes;

namespace AgentTest
{
    [Agent]
    [Environement("env2")]
    [Belief("k(p)")]
    public class myAgent : Agent
    {
        public override void onInit()
        {
            
            AddEvent("delivered(product,quantity,ID)", AgentPerceptionType.AddBelief, typeof(get));

            //AddEvent("f(3)", AgentPerception.AddBelief, typeof(myPlan));       


            /*PlanArgs args = new PlanArgs();
            args.Add("name", "davide");
            AchieveGoal(typeof(get),args);*/





        }


        [Plan]//("has(\"beer\")", typeof(get2))]
        public class get : PlanModel
        {
            [PlanEntryPoint]
            void entry(PlanArgs args)
            {

                string name = args.GetArg<string>("name");

                for (int i = 0; i < 2; i++)
                {
                    Thread.Sleep(i * 1000);
                    Console.WriteLine("Ciao " + name + "(" + i + ")");
                }

                AgentMessage msg = new AgentMessage("get2(\"beer\")", InformationType.Achieve);
                Parent.SendMessage("myAgent", msg);



                //Console.WriteLine("WEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE");
                //.send(robot, achieve, has(owner,beer)).
            }
        }


        [Plan]//("!has(\"beer\")")]
        [Parameter("product")]
        public class get2 : PlanModel
        {
            [PlanEntryPoint]
            void entry(PlanArgs args)
            {
                string product = args.GetArg<string>("product");
                Console.WriteLine("WAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA "+product);
                //.send(robot, achieve, has(owner,beer)).
            }
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
