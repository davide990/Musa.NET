/**
         __  __                                     _   
        |  \/  |                                   | |  
        | \  / | _   _  ___   __ _     _ __    ___ | |_ 
        | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
        | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
        |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|

*/
using AgentLibrary;
using FormulaLibrary;
using Quartz;
using System;

namespace AgentTest
{
    class Program
    {
        static void Main(string[] args)
        {
            LiteralTerm a = new LiteralTerm("a");
            Console.WriteLine("ho creato " + a.ToString());

            VariableTerm<int> var_a = a.toVariableTerm(5);
            Console.WriteLine("ho convertito " + a.ToString() + " in " + var_a.ToString());

            Term aaa = a.toVariableTerm(5);
            Console.WriteLine("aaa è " + aaa.GetType().ToString());

            aaa = ((VariableTerm<int>)aaa).toLiteralTerm();
            Console.WriteLine("aaa è " + aaa.GetType().ToString());


            Console.ReadKey(true);
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
