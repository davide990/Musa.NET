﻿/**
         __  __                                     _   
        |  \/  |                                   | |  
        | \  / | _   _  ___   __ _     _ __    ___ | |_ 
        | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
        | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
        |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|

*/
using FormulaLibrary;
using FormulaLibrary.ANTLR;
using NUnit.Framework;
using MusaCommon;

namespace FormulaLibraryTest.ANTLRTest
{
    [TestFixture]
    class CompositeFormulaParserTest
    {
        [TestCase(Result = true)]
        [Test]
        public bool formula_test_0()
        {
            var FormulaParser = new FormulaParser();
            string formula = "!f(x)";

            IFormula generatedFormula = FormulaParser.Parse(formula);

            Formula n = new AtomicFormula("f", new LiteralTerm("x"));
            Formula expectedFormula = new NotFormula(n);

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(Result = true)]
        [Test]
        public bool formula_test_1()
        {
            var FormulaParser = new FormulaParser();
            string formula = "f(x)|h(s)";

            IFormula generatedFormula = FormulaParser.Parse(formula);

            Formula a = new AtomicFormula("f", new LiteralTerm("x"));
            Formula b = new AtomicFormula("h", new LiteralTerm("s"));
            Formula expectedFormula = new OrFormula(a, b);

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(Result = true)]
        [Test]
        public bool formula_test_2()
        {
            var FormulaParser = new FormulaParser();
            string formula = "f(x)&h(s)";

            IFormula generatedFormula = FormulaParser.Parse(formula);

            Formula a = new AtomicFormula("f", new LiteralTerm("x"));
            Formula b = new AtomicFormula("h", new LiteralTerm("s"));
            Formula expectedFormula = new AndFormula(a, b);

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(Result = true)]
        [Test]
        public bool formula_test_3()
        {
            var FormulaParser = new FormulaParser();
            string formula = "f(x) &(!h(s))";

            IFormula generatedFormula = FormulaParser.Parse(formula);

            Formula a = new AtomicFormula("f", new LiteralTerm("x"));
            Formula b = new AtomicFormula("h", new LiteralTerm("s"));
            Formula b_n = new NotFormula(b);
            Formula expectedFormula = new AndFormula(a, b_n);

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(Result = true)]
        [Test]
        public bool formula_test_4()
        {
            var FormulaParser = new FormulaParser();
            string formula = "(!f(x)   | h(s))";
            IFormula generatedFormula = FormulaParser.Parse(formula);

            Formula a = new AtomicFormula("f", new LiteralTerm("x"));
            Formula a_n = new NotFormula(a);
            Formula b = new AtomicFormula("h", new LiteralTerm("s"));
            Formula expectedFormula = new OrFormula(a_n, b);

            return generatedFormula.Equals(expectedFormula);
        }


        [TestCase(Result = true)]
        [Test]
        public bool formula_test_5()
        {
            var FormulaParser = new FormulaParser();
            string formula = "!(f(x)   | h(s))";
            
            IFormula generatedFormula = FormulaParser.Parse(formula);

            Formula a = new AtomicFormula("f", new LiteralTerm("x"));
            Formula b = new AtomicFormula("h", new LiteralTerm("s"));
            Formula expectedFormula = new NotFormula(new OrFormula(a, b));

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(Result = false)]
        [Test]
        public bool formula_test_6()
        {
            var FormulaParser = new FormulaParser();
            string formula = "!(f(x))   | h(s)";
            IFormula generatedFormula = FormulaParser.Parse(formula);

            Formula a = new AtomicFormula("f", new LiteralTerm("x"));
            Formula b = new AtomicFormula("h", new LiteralTerm("s"));
            Formula expectedFormula = new NotFormula(new OrFormula(a, b));

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(Result = false)]
        [Test]
        public bool formula_test_7()
        {
            var FormulaParser = new FormulaParser();
            string formula = "(!(f(x)) | h(s))";
            IFormula generatedFormula = FormulaParser.Parse(formula);

            Formula a = new AtomicFormula("f", new LiteralTerm("x"));
            Formula b = new AtomicFormula("h", new LiteralTerm("s"));
            Formula expectedFormula = new NotFormula(new OrFormula(a, b));

            return generatedFormula.Equals(expectedFormula);
        }

        /// composite
        

        [TestCase(Result = true)]
        [Test]
        public bool formula_test_8()
        {
            var FormulaParser = new FormulaParser();
            string formula = "(f(x)|h(s))&o(m)";
            IFormula generatedFormula = FormulaParser.Parse(formula);

            Formula a = new AtomicFormula("f", new LiteralTerm("x"));
            Formula b = new AtomicFormula("h", new LiteralTerm("s"));
            Formula c = new AtomicFormula("o", new LiteralTerm("m"));
            Formula expectedFormula = new AndFormula(new OrFormula(a, b), c);

            return generatedFormula.Equals(expectedFormula);
        }



        [TestCase(Result = true)]
        [Test]
        public bool formula_test_9()
        {
            var FormulaParser = new FormulaParser();
            string formula = "(f(x)&h(s))|o(m)";
            IFormula generatedFormula = FormulaParser.Parse(formula);

            Formula a = new AtomicFormula("f", new LiteralTerm("x"));
            Formula b = new AtomicFormula("h", new LiteralTerm("s"));
            Formula c = new AtomicFormula("o", new LiteralTerm("m"));
            Formula expectedFormula = new OrFormula(new AndFormula(a, b), c);

            return generatedFormula.Equals(expectedFormula);
        }


        [TestCase(Result = true)]
        [Test]
        public bool formula_test_10()
        {
            var FormulaParser = new FormulaParser();
            string formula = "(f(x)&!h(s))|o(m)";
            IFormula generatedFormula = FormulaParser.Parse(formula);

            Formula a = new AtomicFormula("f", new LiteralTerm("x"));
            Formula b = new NotFormula(new AtomicFormula("h", new LiteralTerm("s")));
            Formula c = new AtomicFormula("o", new LiteralTerm("m"));
            Formula expectedFormula = new OrFormula(new AndFormula(a, b), c);

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(Result = true)]
        [Test]
        public bool formula_test_11()
        {
            var FormulaParser = new FormulaParser();
            string formula = "(f(x)&!h(s))|!o(m)";
            IFormula generatedFormula = FormulaParser.Parse(formula);

            Formula a = new AtomicFormula("f", new LiteralTerm("x"));
            Formula b = new NotFormula(new AtomicFormula("h", new LiteralTerm("s")));
            Formula c = new NotFormula(new AtomicFormula("o", new LiteralTerm("m")));
            Formula expectedFormula = new OrFormula(new AndFormula(a, b), c);

            return generatedFormula.Equals(expectedFormula);
        }


        [TestCase(Result = true)]
        [Test]
        public bool formula_test_12()
        {
            var FormulaParser = new FormulaParser();
            string formula = "(   f(x,k<-int(3))&!h(s,o,a<-string(\"ciao mondo\")))  |!o(m,s<-char('d'))";
            IFormula generatedFormula = FormulaParser.Parse(formula);

            Formula a = new AtomicFormula("f", new LiteralTerm("x"), new VariableTerm<int>("k",3));
            Formula b = new NotFormula(new AtomicFormula("h", new LiteralTerm("s"), new LiteralTerm("o"), new VariableTerm<string>("a", "ciao mondo")));
            Formula c = new NotFormula(new AtomicFormula("o", new LiteralTerm("m"), new VariableTerm<char>("s",'d')));
            Formula ab = new AndFormula(a, b);
            
            Formula expectedFormula = new OrFormula(ab, c);

            return generatedFormula.Equals(expectedFormula);
        }
    }
}
