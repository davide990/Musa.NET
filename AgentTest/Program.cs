﻿/**
         __  __                                     _   
        |  \/  |                                   | |  
        | \  / | _   _  ___   __ _     _ __    ___ | |_ 
        | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
        | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
        |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
*/
using AgentLibrary;
using System;
using System.Threading;
using MusaConfiguration;
using MusaLogger;
using FormulaLibrary;
using System.ComponentModel;
using PlanLibrary;


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

		static void configureAndStartMusa()
		{
			AgentEnvironement env = AgentEnvironement.GetInstance();
			
            Agent a = new Agent ("agent_1").Start();

			BackgroundWorker wk = new BackgroundWorker ();
			wk.DoWork += delegate 
			{
				Thread.Sleep(3000);
                Console.WriteLine("Add f(x)");
				env.RegisterStatement (new AtomicFormula ("f", new LiteralTerm ("x")));

				//Thread.Sleep(2000);
                //Console.WriteLine("Remove f(x)");
				//env.DeleteStatement (new AtomicFormula ("f", new LiteralTerm ("x")));

			};
			wk.RunWorkerAsync ();

			a.AddPlan (typeof(PlanExample));
			a.AddPlan (typeof(PlanExample2));
			a.AddPlan (typeof(PlanForEvent));

            var argss = new AgentEventArgs { { "nome", "davide" } };
                
            a.AddEvent ("f(x)", AgentPerception.AddBelief, typeof(PlanExample2));
            //a.AddEvent ("f(x)", AgentPerception.RemoveBelief, typeof(PlanExample2));
            env.RegisterAgent (a);

			//env.RegisterStatement (new AtomicFormula ("f", new LiteralTerm ("x")));
			//a.AchieveGoal (typeof(PlanExample2));

            //env.Serialize().Save("/home/davide/ehyehy.xml");

			//TODO implementare un meccanismo di attesa per tutti gli agenti registrati nel sistema
			env.WaitForAgents ();
		}

        static void Main(string[] args)
        {
            //startMUSA();

			MusaConfig.ReadFromFile ("../../test_conf.xml");
            //AgentEnvironement env = AgentEnvironement.GetInstance();
            //env.RegisterAgentFromConfiguration();

            //env.WaitForAgents();

			/*
			BackgroundWorker wk = new BackgroundWorker ();
			wk.DoWork += delegate 
			{
				
				MusaConfig.GetLoggerSet ().Log (LogLevel.Info, "hello from MUSA bg");
			};
			wk.RunWorkerAsync ();

			Thread.Sleep (10000);
			*/
            /*
            AgentEnvironement env = AgentEnvironement.GetInstance();
            env.RegisterAgentFromConfiguration();
            var a = env.RegisteredAgents;
            */

			configureAndStartMusa ();
        }

		static void A_RegisterResult (string result)
		{
			Console.WriteLine ("PRODUCED RESULT: " + result);
		}

    }

	[Plan]
    class PlanForEvent : PlanModel
	{
		[PlanEntryPoint]
        void entry_point(AgentEventArgs args)
		{
			object a;
			args.TryGetValue ("nome",out a);

			Console.WriteLine ("Hello from " + EntryPointName + " to " + a);
		}
	}



	[Plan]
    public class PlanExample2 : PlanModel
	{
		[PlanEntryPoint]
		void wella(AgentEventArgs args)
		{
            object a;
            args.TryGetValue ("nome",out a);

            Console.WriteLine("ECCOMI, MALEDIZIONE");

			//log (LogLevel.Info, "Ciao LOGGERRRRRRRRRRRRRRRRRRR");

            if (a != null)
                log(LogLevel.Info, "Hello from " + a);

			//ExecuteStep ("other_step1");
		}

		[PlanStep]
		void other_step1()
		{
			log (LogLevel.Info, "Ciao 2");
			Thread.Sleep (500);

			log (LogLevel.Info, "Ciao 3");
			Thread.Sleep (500);
			//RegisterResult ("f(x)");
		}
	}

	//[AtomicPlan]
	[Plan]
    class PlanExample : PlanModel
	{
		[PlanEntryPoint]
		void entry_point(AgentEventArgs args)
		{
			//object a;
			//args.TryGetValue ("nome",out a);

			//Console.WriteLine ("Hello from " + EntryPointName + " to " + a.ToString());
			Console.WriteLine ("Hello plan =)");

            var the_args = new AgentEventArgs (){ { "nome", "davide" } };


            ExecuteStep ("wella", the_args);
		}

		[PlanStep]
		void wella(AgentEventArgs args)
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
