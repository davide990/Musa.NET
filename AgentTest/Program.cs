/**
         __  __                                     _   
        |  \/  |                                   | |  
        | \  / | _   _  ___   __ _     _ __    ___ | |_ 
        | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
        | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
        |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|

*/
using AgentLibrary;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using FormulaLibrary;
using FormulaLibrary.ANTLR;
using FormulaLibrary.ANTLR.visitor;
using Quartz;
using System;
using System.IO;
using System.Text;

namespace AgentTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //AgentWorkbench wb = new AgentWorkbench(a);
            /*wb.addStatement(FormulaParser.Parse("y(x<-int(3))") as AtomicFormula,
                            FormulaParser.Parse("f(x)") as AtomicFormula,
                            FormulaParser.Parse("h(s,o,a<-string(\"ciao mondo\"))") as AtomicFormula,
                            FormulaParser.Parse("o(m,s<-char('d')") as AtomicFormula);*/

            AgentEnvironement env = new AgentEnvironement("8080");
            Agent a = new Agent("agent_1").start();
            Agent b = new Agent("agent_2").start();

            env.RegisterAgent(a);
            env.RegisterAgent(b);


            env.RegisterStatement(FormulaParser.Parse("f(x)") as AtomicFormula);
            


            Console.WriteLine(a.Workbench.ToString());


            Console.ReadKey();
        }


        
        private void quartz_test_0()
        {
            Agent worker = new Agent("worker");

            IJobDetail jobDetail = JobBuilder.Create<testJob1>()
                                            .WithIdentity("myJob", "group1") // name "myJob", group "group1"
                                            .Build();

            ITrigger trigger = TriggerBuilder.Create()
                               .WithIdentity("myTrigger", "group1")
                               .StartNow()
                               .WithSimpleSchedule(x => x
                                   .WithIntervalInSeconds(10)
                                   .RepeatForever())
                               .Build();

            worker.start();
            
            worker.scheduleJob(trigger, jobDetail);

        }
    }


    class testJob1 : AgentJob
    {
        public testJob1() : base("ciao")
        {
            
        }

        public override void Execute(IJobExecutionContext context)
        {
            System.Console.WriteLine("Hello from test job!");
        }
    }
}
