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
using System;
using System.Collections.Generic;
using PlanLibrary;
using System.Threading;
using System.ComponentModel;

namespace AgentTest
{
	class Program
    {
        private static void startMUSA()
        {
            AgentEnvironement env = AgentEnvironement.GetInstance();
			Agent a = new Agent("agent_1").Start();
			Agent b = new Agent("agent_2").Start();
			Agent c = new Agent("agent_3").Start();
			env.RegisterAgent(a);
			env.RegisterAgent(b);
			env.RegisterAgent(c);
        }

        static void Main(string[] args)
        {
            //startMUSA();



			AgentEnvironement env = AgentEnvironement.GetInstance();
			Agent a = new Agent ("agent_1").Start();

			BackgroundWorker wk = new BackgroundWorker ();
			wk.DoWork += delegate 
			{
				Thread.Sleep(10000);
				a.Pause();
				Thread.Sleep(20000);
				a.Resume();
			};
			wk.RunWorkerAsync ();

			a.AddPlan (typeof(PlanExample));
			a.AddEvent ("f(x)", PerceptionType.AddBelief, typeof(PlanExample));

			env.RegisterAgent (a);

			env.RegisterStatement (new AtomicFormula ("f", new LiteralTerm ("x")));



			//a.ExecutePlan (typeof(PlanExample2));

			//Console.WriteLine ("Executing plan " + a.CurrentExecutingPlan);
			while (a.Busy);

			Console.ReadKey();
        }

		static void A_RegisterResult (string result)
		{
			Console.WriteLine ("PRODUCED RESULT: " + result);
		}

    }

	[Plan]
	class PlanExample2 : PlanModel
	{
		[PlanEntryPoint]
		void wella(Dictionary<string,object> args)
		{
			Console.WriteLine ("Ciao!");
			ExecuteStep ("other_step1");
		}

		[PlanStep]
		void other_step1()
		{
			Console.WriteLine ("Ciao 2");
			Thread.Sleep (500);

			Console.WriteLine ("Ciao 3");
			Thread.Sleep (50000);

			//RegisterResult ("f(x)");
		}
	}


	[Plan]
	class PlanExample : PlanModel
	{
		[PlanEntryPoint]
		void entry_point(Dictionary<string,object> args)
		{
			//object a;
			//args.TryGetValue ("nome",out a);

			//Console.WriteLine ("Hello from " + EntryPointName + " to " + a.ToString());
			Console.WriteLine ("Hello plan =)");

			ExecuteStep ("wella", new Dictionary<string, object> (){ { "nome", "davide" } });
		}


		[PlanStep]
		void wella(Dictionary<string,object> args)
		{
			object a;
			args.TryGetValue ("nome",out a);

			Console.WriteLine ("working...");
			Thread.Sleep (2000);
			Console.WriteLine ("Hello "+a.ToString() + " from plan step "+PlanStepName);
			RegisterResult ("hi 1");
			ExecuteStep ("intensive_task1");
		}

		[PlanStep]
		void intensive_task1()
		{
			Thread.Sleep (2000);
			Console.WriteLine (PlanStepName + " completed!");
			ExecuteStep ("intensive_task2");
		}

		[PlanStep]
		void intensive_task2()
		{
			Thread.Sleep (2000);
			Console.WriteLine (PlanStepName + " completed!");
			RegisterResult ("hi 3");
			ExecuteStep ("intensive_task3");

		}

		[PlanStep]
		void intensive_task3()
		{
			Thread.Sleep (2000);
			Console.WriteLine (PlanStepName + " completed!");
			ExecuteStep ("intensive_task4");
		}

		[PlanStep]
		void intensive_task4()
		{
			Thread.Sleep (2000);
			Console.WriteLine (PlanStepName + " completed!");
			ExecuteStep ("intensive_task5");
		}

		[PlanStep]
		void intensive_task5()
		{
			Thread.Sleep (2000);
			Console.WriteLine (PlanStepName + " completed!");
			ExecuteStep ("intensive_task6");
		}

		[PlanStep]
		void intensive_task6()
		{
			Thread.Sleep (2000);
			Console.WriteLine (PlanStepName + " completed!");
			RegisterResult ("hi 7");
			ExecuteStep ("intensive_task7");

		}

		[PlanStep]
		void intensive_task7()
		{
			Thread.Sleep (2000);
			Console.WriteLine (PlanStepName + " completed!");
			ExecuteStep ("intensive_task8");
		}

		[PlanStep]
		void intensive_task8()
		{
			Thread.Sleep (2000);
			Console.WriteLine (PlanStepName + " completed!");
			ExecuteStep ("intensive_task9");
		}

		[PlanStep]
		void intensive_task9()
		{
			Thread.Sleep (2000);
			Console.WriteLine (PlanStepName + " completed!");
			ExecuteStep ("intensive_task10");
		}

		[PlanStep]
		void intensive_task10()
		{
			Thread.Sleep (2000);
			Console.WriteLine (PlanStepName + " completed! ALL DONE");
		}

	}

        
}
