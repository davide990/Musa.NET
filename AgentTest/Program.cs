/**
         __  __                                     _   
        |  \/  |                                   | |  
        | \  / | _   _  ___   __ _     _ __    ___ | |_ 
        | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
        | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
        |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|

*/
using AgentLibrary;
using AgentLibrary.Networking;
using FormulaLibrary;
using FormulaLibrary.ANTLR;
using Quartz;
using System;
using System.Collections.Generic;

namespace AgentTest
{
    class Program
    {

        private static void startMUSA()
        {
            AgentEnvironement env = AgentEnvironement.GetInstance();
			Agent a = new Agent("agent_1").start();
			Agent b = new Agent("agent_2").start();
			Agent c = new Agent("agent_3").start();
			env.RegisterAgent(a);
			env.RegisterAgent(b);
			env.RegisterAgent(c);
        }

        static void Main(string[] args)
        {
            startMUSA();


            //FormulaGenerator fg = new FormulaGenerator(2, 2, 2, 1, true);

            //for (int i = 0; i < 50; i++)
            //{
            //    Console.WriteLine(FormulaParser.Parse(fg.GetRandomFormula().ToString()).ToString());
            //    //Console.WriteLine(fg.GetRandomFormula().ToString());
            //}


            //AgentEnvironement env = new AgentEnvironement();
            //Agent a = new Agent("agent_1").start();
            ////Agent b = new Agent("agent_2");

            //env.RegisterAgent(a);

            //EnvironmentServer srv = new EnvironmentServer(env);
            //srv.StartNetworking("8080");

            //env.RegisterAgent(b);

            //env.RegisterStatement(FormulaParser.Parse("f(x)") as AtomicFormula);

            //Console.WriteLine(a.Workbench.ToString());


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
