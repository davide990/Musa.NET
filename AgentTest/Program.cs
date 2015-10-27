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
			a.SetArgs (new Dictionary<string, object> (){ { "nome", "davide" } });

			a.Execute ();
            Console.ReadKey();
        }
    }

	[Plan]
	class PlanExample : PlanModel
	{
		[PlanEntryPoint]
		void a(Dictionary<string,object> args)
		{
			object a;
			args.TryGetValue ("nome",out a);

			Console.WriteLine ("Hello from " + Name + " to " + a.ToString());

			ExecuteStep ("wella");
		}


		[PlanStep]
		void wella(Dictionary<string,object> args)
		{
			Console.WriteLine ("Hello");
		}

	}
}
