//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  MyClass.cs
//
//  Author:
//       Davide Guastella <davide.guastella90@gmail.com>
//
//  Copyright (c) 2015 Davide Guastella
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Threading;
using PlanLibrary;
using MusaCommon;

namespace PlanLibraryExample
{
    public class MyClass
    {
    }

    [Plan]
    public class PlanForEvent : PlanModel
    {
        [PlanEntryPoint]
        void entry_point(IAgentEventArgs args)
        {
            string a;
            args.TryGetValue ("nome",out a);

            Console.WriteLine ("Hello from " + EntryPointName + " to " + a.ToString());
        }
    }



    [Plan]
    public class PlanExample2 : PlanModel
    {
        [PlanEntryPoint]
        void wella(IAgentEventArgs args)
        {
            string a;
            args.TryGetValue ("nome",out a);

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
        void entry_point(IAgentEventArgs args)
        {
            //object a;
            //args.TryGetValue ("nome",out a);

            //Console.WriteLine ("Hello from " + EntryPointName + " to " + a.ToString());
            Console.WriteLine ("Hello plan =)");

            var the_args = new PlanArgs (){ { "nome", "davide" } };

            ExecuteStep ("wella", the_args);
        }

        [PlanStep]
        void wella(IAgentEventArgs args)
        {
            string a;
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

