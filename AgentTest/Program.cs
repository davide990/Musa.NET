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
using PlanLibrary;
using System.Threading;

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
            //startMUSA();

			PlanInstance<PlanExample> a = new PlanInstance<PlanExample> ();

			a.Execute (new Dictionary<string, object> (){ { "nome", "davide" } });

			while (!a.HasFinished);
            
			Console.ReadKey();
        }
    }

	[Plan]
	class PlanExample : PlanModel
	{
		[PlanEntryPoint]
		void entry_point(Dictionary<string,object> args)
		{
			object a;
			args.TryGetValue ("nome",out a);

			Console.WriteLine ("Hello from " + Name + " to " + a.ToString());

			ExecuteStep ("wella", new Dictionary<string, object> (){ { "nome", "davide" } });
		}


		[PlanStep]
		void wella(Dictionary<string,object> args)
		{
			object a;
			args.TryGetValue ("nome",out a);

			Console.WriteLine ("working...");
			Thread.Sleep (5000);
			Console.WriteLine ("Hello "+a.ToString() + " from plan step "+PlanStepName);
			ExecuteStep ("intensive_task1");
		}

		[PlanStep]
		void intensive_task1()
		{
			Thread.Sleep (9000);
			Console.WriteLine ("Intensive task #1 completed!");
			ExecuteStep ("intensive_task2");
		}

		[PlanStep]
		void intensive_task2()
		{
			Thread.Sleep (9000);
			Console.WriteLine ("Intensive task #2 completed!");
			ExecuteStep ("intensive_task3");
		}

		[PlanStep]
		void intensive_task3()
		{
			Thread.Sleep (9000);
			Console.WriteLine ("Intensive task #3 completed!");
			ExecuteStep ("intensive_task4");
		}

		[PlanStep]
		void intensive_task4()
		{
			Thread.Sleep (9000);
			Console.WriteLine ("Intensive task #4 completed! ALL DONE");
		}

	}
}
