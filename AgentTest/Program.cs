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
            //AtomicFormula a = new AtomicFormula("f", new LiteralTerm("x"), new VariableTerm<short>("y", 1));
            //Console.WriteLine(a.ToString(false));
            //Console.ReadKey(true);
            visitor_test();
        }



        private static void visitor_test()
        {
            // convert string to stream
            //byte[] byteArray = Encoding.ASCII.GetBytes("!f(x,k<-int(2))");
            byte[] byteArray = Encoding.ASCII.GetBytes("f(x,k)");
            MemoryStream m_stream = new MemoryStream(byteArray);

            // convert stream to string
            StreamReader reader = new StreamReader(m_stream);

            AntlrInputStream stream = new AntlrInputStream(reader);
            ITokenSource lexer = new formula_grammarLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            formula_grammarParser parser = new formula_grammarParser(tokens);
            parser.BuildParseTree = true;


            IParseTree tree = parser.disjunction();
            FormulaVisitor vv = new FormulaVisitor();


            Formula ffff = vv.Visit(tree);


            Console.WriteLine(vv.Visit(tree));
            
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
