//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  CompositeFormulaParserTest.cs
//
//  Author:
//       Davide Guastella <davide.guastella90@gmail.com>
//
//  Copyright (c) 2016 Davide Guastella
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

using FormulaLibrary;
using NUnit.Framework;
using MusaCommon;

namespace FormulaLibraryTestProject
{
    [TestFixture]
    class CompositeFormulaParserTest
    {
        [TestCase(ExpectedResult = true)]
        [Test]
        public bool formula_test_0()
        {
            var FormulaUtils = new FormulaUtils();
            string formula = "!f(x)";

            IFormula generatedFormula = FormulaUtils.Parse(formula);

            Formula n = new AtomicFormula("f", new LiteralTerm("x"));
            Formula expectedFormula = new NotFormula(n);

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(ExpectedResult = true)]
        [Test]
        public bool formula_test_1()
        {
            var FormulaUtils = new FormulaUtils();
            string formula = "f(x)|h(s)";

            IFormula generatedFormula = FormulaUtils.Parse(formula);

            Formula a = new AtomicFormula("f", new LiteralTerm("x"));
            Formula b = new AtomicFormula("h", new LiteralTerm("s"));
            Formula expectedFormula = new OrFormula(a, b);

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(ExpectedResult = true)]
        [Test]
        public bool formula_test_2()
        {
            var FormulaUtils = new FormulaUtils();
            string formula = "f(x)&h(s)";

            IFormula generatedFormula = FormulaUtils.Parse(formula);

            Formula a = new AtomicFormula("f", new LiteralTerm("x"));
            Formula b = new AtomicFormula("h", new LiteralTerm("s"));
            Formula expectedFormula = new AndFormula(a, b);

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(ExpectedResult = true)]
        [Test]
        public bool formula_test_3()
        {
            var FormulaUtils = new FormulaUtils();
            string formula = "f(x) &(!h(s))";

            IFormula generatedFormula = FormulaUtils.Parse(formula);

            Formula a = new AtomicFormula("f", new LiteralTerm("x"));
            Formula b = new AtomicFormula("h", new LiteralTerm("s"));
            Formula b_n = new NotFormula(b);
            Formula expectedFormula = new AndFormula(a, b_n);

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(ExpectedResult = true)]
        [Test]
        public bool formula_test_4()
        {
            var FormulaUtils = new FormulaUtils();
            string formula = "(!f(x)   | h(s))";
            IFormula generatedFormula = FormulaUtils.Parse(formula);

            Formula a = new AtomicFormula("f", new LiteralTerm("x"));
            Formula a_n = new NotFormula(a);
            Formula b = new AtomicFormula("h", new LiteralTerm("s"));
            Formula expectedFormula = new OrFormula(a_n, b);

            return generatedFormula.Equals(expectedFormula);
        }


        [TestCase(ExpectedResult = true)]
        [Test]
        public bool formula_test_5()
        {
            var FormulaUtils = new FormulaUtils();
            string formula = "!(f(x)   | h(s))";
            
            IFormula generatedFormula = FormulaUtils.Parse(formula);

            Formula a = new AtomicFormula("f", new LiteralTerm("x"));
            Formula b = new AtomicFormula("h", new LiteralTerm("s"));
            Formula expectedFormula = new NotFormula(new OrFormula(a, b));

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(ExpectedResult = false)]
        [Test]
        public bool formula_test_6()
        {
            var FormulaUtils = new FormulaUtils();
            string formula = "!(f(x))   | h(s)";
            IFormula generatedFormula = FormulaUtils.Parse(formula);

            Formula a = new AtomicFormula("f", new LiteralTerm("x"));
            Formula b = new AtomicFormula("h", new LiteralTerm("s"));
            Formula expectedFormula = new NotFormula(new OrFormula(a, b));

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(ExpectedResult = false)]
        [Test]
        public bool formula_test_7()
        {
            var FormulaUtils = new FormulaUtils();
            string formula = "(!(f(x)) | h(s))";
            IFormula generatedFormula = FormulaUtils.Parse(formula);

            Formula a = new AtomicFormula("f", new LiteralTerm("x"));
            Formula b = new AtomicFormula("h", new LiteralTerm("s"));
            Formula expectedFormula = new NotFormula(new OrFormula(a, b));

            return generatedFormula.Equals(expectedFormula);
        }

        /// composite
        

        [TestCase(ExpectedResult = true)]
        [Test]
        public bool formula_test_8()
        {
            var FormulaUtils = new FormulaUtils();
            string formula = "(f(x)|h(s))&o(m)";
            IFormula generatedFormula = FormulaUtils.Parse(formula);

            Formula a = new AtomicFormula("f", new LiteralTerm("x"));
            Formula b = new AtomicFormula("h", new LiteralTerm("s"));
            Formula c = new AtomicFormula("o", new LiteralTerm("m"));
            Formula expectedFormula = new AndFormula(new OrFormula(a, b), c);

            return generatedFormula.Equals(expectedFormula);
        }



        [TestCase(ExpectedResult = true)]
        [Test]
        public bool formula_test_9()
        {
            var FormulaUtils = new FormulaUtils();
            string formula = "(f(x)&h(s))|o(m)";
            IFormula generatedFormula = FormulaUtils.Parse(formula);

            Formula a = new AtomicFormula("f", new LiteralTerm("x"));
            Formula b = new AtomicFormula("h", new LiteralTerm("s"));
            Formula c = new AtomicFormula("o", new LiteralTerm("m"));
            Formula expectedFormula = new OrFormula(new AndFormula(a, b), c);

            return generatedFormula.Equals(expectedFormula);
        }


        [TestCase(ExpectedResult = true)]
        [Test]
        public bool formula_test_10()
        {
            var FormulaUtils = new FormulaUtils();
            string formula = "(f(x)&!h(s))|o(m)";
            IFormula generatedFormula = FormulaUtils.Parse(formula);

            Formula a = new AtomicFormula("f", new LiteralTerm("x"));
            Formula b = new NotFormula(new AtomicFormula("h", new LiteralTerm("s")));
            Formula c = new AtomicFormula("o", new LiteralTerm("m"));
            Formula expectedFormula = new OrFormula(new AndFormula(a, b), c);

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(ExpectedResult = true)]
        [Test]
        public bool formula_test_11()
        {
            var FormulaUtils = new FormulaUtils();
            string formula = "(f(x)&!h(s))|!o(m)";
            IFormula generatedFormula = FormulaUtils.Parse(formula);

            Formula a = new AtomicFormula("f", new LiteralTerm("x"));
            Formula b = new NotFormula(new AtomicFormula("h", new LiteralTerm("s")));
            Formula c = new NotFormula(new AtomicFormula("o", new LiteralTerm("m")));
            Formula expectedFormula = new OrFormula(new AndFormula(a, b), c);

            return generatedFormula.Equals(expectedFormula);
        }


        [TestCase(ExpectedResult = true)]
        [Test]
        public bool formula_test_12()
        {
            var FormulaUtils = new FormulaUtils();
            string formula = "(   f(x,k<-int(3))&!h(s,o,a<-string(\"ciao mondo\")))  |!o(m,s<-char('d'))";
            IFormula generatedFormula = FormulaUtils.Parse(formula);

            Formula a = new AtomicFormula("f", new LiteralTerm("x"), new VariableTerm<int>("k", 3));
            Formula b = new NotFormula(new AtomicFormula("h", new LiteralTerm("s"), new LiteralTerm("o"), new VariableTerm<string>("a", "ciao mondo")));
            Formula c = new NotFormula(new AtomicFormula("o", new LiteralTerm("m"), new VariableTerm<char>("s", 'd')));
            Formula ab = new AndFormula(a, b);
            
            Formula expectedFormula = new OrFormula(ab, c);

            return generatedFormula.Equals(expectedFormula);
        }
    }
}
