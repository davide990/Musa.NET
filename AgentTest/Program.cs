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
using MusaCommon;
using PlanLibrary;
using System;
using System.ComponentModel;
using System.Threading;


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
            var FormulaParser = ModuleProvider.Get().Resolve<IFormulaUtils>();

            Agent a = new Agent("agent_1").Start();
            Agent b = new Agent("agent_2").Start();
            //Agent ag_b = new Agent ("agent_2").Start();

            BackgroundWorker wk = new BackgroundWorker();
            wk.DoWork += delegate
            {
                
                /*Thread.Sleep(5000);
                Console.WriteLine("Add f(x)");
                //env.RegisterStatement(new AtomicFormula("f", new LiteralTerm("x")));
                env.RegisterStatement(new AtomicFormula("f", new ValuedTerm<int>(3)));
                Thread.Sleep(4000);
                Console.WriteLine("Remove f(x)");
                env.DeleteStatement(new AtomicFormula("f", new ValuedTerm<int>(3)));
                */
                Thread.Sleep(5000);
                AgentMessage mm = new AgentMessage();
                mm.Sender = "agent_2";

                /*mm.AddInfo(FormulaParser.Parse("f(x)"));
                mm.InfoType = InformationType.AskOne;*/

                mm.AddInfo(FormulaParser.Parse("(f(x)|k(x))|(w(hello)&l(p))"));
                mm.InfoType = InformationType.Tell;

                b.SendMessage("agent_1", mm);

            };
            wk.RunWorkerAsync();

            /*a.AddPlan(typeof(PlanExample));
            a.AddPlan(typeof(PlanExample2));
            a.AddPlan(typeof(PlanForEvent));*/
            a.AddPlan(typeof(HelloWorldPlan));

            var argss = new PlanArgs { { "nome", "davide" } };
            a.AddEvent("f(3)", AgentPerception.AddBelief, typeof(HelloWorldPlan), argss);

            //a.AddEvent ("f(x)", AgentPerception.RemoveBelief, typeof(PlanExample2));
            env.RegisterAgent(a);
            env.RegisterAgent(b);
            //env.RegisterAgent (ag_b);

            a.AddBelief(new AtomicFormula("f", new ValuedTerm<int>(3)));
            

            //a.AddBelief(FormulaParser.Parse("k(x)"), FormulaParser.Parse("p(3)"));
           // var ll = new List<IFormula> { FormulaParser.Parse("w(\"hello\")"), FormulaParser.Parse("w(x)"), FormulaParser.Parse("cc(x)") };
            //a.AddBelief(ll);


            //env.RegisterStatement (new AtomicFormula ("f", new LiteralTerm ("x")));
            //a.AchieveGoal (typeof(PlanExample2));


            //a.AchieveGoal(typeof(PlanForEvent), argss);

            //env.Serialize().Save("/home/davide/ehyehy.xml");

            //TODO implementare un meccanismo di attesa per tutti gli agenti registrati nel sistema
            env.WaitForAgents();
        }

        static void Main(string[] args)
        {
            //startMUSA();

            MusaInitializer.MusaInitializer.Initialize();

            //MusaConfig.ReadFrom/File("../../test_conf.xml");
            var logger = ModuleProvider.Get().Resolve<ILogger>();
            logger.AddFragment<IConsoleLoggerFragment>(LogLevel.Trace);
            //logger.GetFragment<IConsoleLoggerFragment>().SetMinimumLogLevel(LogLevel.Debug);

            //var fp = ModuleProvider.Get().Resolve<IFormulaUtils>();
            configureAndStartMusa();
        }

        static void A_RegisterResult(string result)
        {
            Console.WriteLine("PRODUCED RESULT: " + result);
        }

    }

    [Plan]
    class PlanForEvent : PlanModel
    {
        [PlanEntryPoint]
        void entry_point(PlanArgs args)
        {
            object a;
            args.TryGetValue("nome", out a);

            Console.WriteLine("Hello from " + EntryPointName + " to " + a);
        }
    }

    [Plan]
    [PlanStepsOrder("hello4", "hello2", "hello3")]
    public class HelloWorldPlan : PlanModel
    {
        [PlanEntryPoint]
        void hello(PlanArgs args)
        {
            log(LogLevel.Info, "~~~~~~HELLO WORLD~~~~~~");
        }

        [PlanStep("f(x)")]
        void hello2(PlanArgs args)
        {
            log(LogLevel.Info, "~~~~~~HELLO WORLD2~~~~~~");
        }

        [PlanStep]
        void hello3(PlanArgs args)
        {
            log(LogLevel.Info, "~~~~~~HELLO WORLD3~~~~~~");
        }

        [PlanStep]
        void hello4(PlanArgs args)
        {
            log(LogLevel.Info, "~~~~~~HELLO WORLD4~~~~~~");
        }
    }


    [Plan]
    public class PlanExample2 : PlanModel
    {
        [PlanEntryPoint]
        void wella(PlanArgs args)
        {
            object a;
            args.TryGetValue("nome", out a);

            Console.WriteLine("ECCOMI, MALEDIZIONE");

            //log (LogLevel.Info, "Ciao LOGGERRRRRRRRRRRRRRRRRRR");

            if (a != null)
                log(LogLevel.Info, "Hello from " + a);

            //ExecuteStep ("other_step1");
        }

        [PlanStep]
        void other_step1()
        {
            log(LogLevel.Info, "Ciao 2");
            Thread.Sleep(500);

            log(LogLevel.Info, "Ciao 3");
            Thread.Sleep(500);
            //RegisterResult ("f(x)");
        }
    }

    //[AtomicPlan]
    [Plan]
    class PlanExample : PlanModel
    {
        [PlanEntryPoint]
        void entry_point(PlanArgs args)
        {
            //object a;
            //args.TryGetValue ("nome",out a);

            Console.WriteLine("Hello plan =)");

            var the_args = new PlanArgs() { { "nome", "davide" } };


            ExecuteStep("wella", the_args);
        }

        [PlanStep]
        void wella(PlanArgs args)
        {
            object a;
            args.TryGetValue("nome", out a);

            Console.WriteLine("working...");
            Thread.Sleep(2000);
            Console.WriteLine("Hello " + a.ToString() + " from plan step " + PlanStepName);
            RegisterResult("hi 1");
            ExecuteStep("intensive_task1");
        }

        [PlanStep]
        void intensive_task1()
        {
            Thread.Sleep(2000);
            Console.WriteLine(PlanStepName + " completed!");
            ExecuteStep("intensive_task2");
        }

        [PlanStep]
        void intensive_task2()
        {
            Thread.Sleep(2000);
            Console.WriteLine(PlanStepName + " completed!");
            RegisterResult("hi 3");
            ExecuteStep("intensive_task3");

        }

        [PlanStep]
        void intensive_task3()
        {
            Thread.Sleep(2000);
            Console.WriteLine(PlanStepName + " completed!");
            ExecuteStep("intensive_task4");
        }

        [PlanStep]
        void intensive_task4()
        {
            Thread.Sleep(2000);
            Console.WriteLine(PlanStepName + " completed!");
            ExecuteStep("intensive_task5");
        }

        [PlanStep]
        void intensive_task5()
        {
            Thread.Sleep(2000);
            Console.WriteLine(PlanStepName + " completed!");
            ExecuteStep("intensive_task6");
        }

        [PlanStep]
        void intensive_task6()
        {
            Thread.Sleep(2000);
            Console.WriteLine(PlanStepName + " completed!");
            RegisterResult("hi 7");
            ExecuteStep("intensive_task7");

        }

        [PlanStep]
        void intensive_task7()
        {
            Thread.Sleep(2000);
            Console.WriteLine(PlanStepName + " completed!");
            ExecuteStep("intensive_task8");
        }

        [PlanStep]
        void intensive_task8()
        {
            Thread.Sleep(2000);
            Console.WriteLine(PlanStepName + " completed!");
            ExecuteStep("intensive_task9");
        }

        [PlanStep]
        void intensive_task9()
        {
            Thread.Sleep(2000);
            Console.WriteLine(PlanStepName + " completed!");
            ExecuteStep("intensive_task10");
        }

        [PlanStep]
        void intensive_task10()
        {
            Thread.Sleep(2000);
            Console.WriteLine(PlanStepName + " completed! ALL DONE");
        }

    }


}
