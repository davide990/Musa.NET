//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  SimpleFormulaParserTest.cs
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
    class SimpleFormulaParserTest
    {
        [TestCase(ExpectedResult = true)]
        [Test]
        public bool formula_test_0()
        {
            string formula = "f(x)";

            IFormula generatedFormula = new FormulaUtils().Parse(formula);
            Formula expectedFormula = new AtomicFormula("f", new LiteralTerm("x"));

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(ExpectedResult = true)]
        [Test]
        public bool formula_test_1()
        {
            string formula = "f(x,y,k)";

            IFormula generatedFormula = new FormulaUtils().Parse(formula);
            Formula expectedFormula = new AtomicFormula("f", new LiteralTerm("x"), new LiteralTerm("y"), new LiteralTerm("k"));

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(ExpectedResult = true)]
        [Test]
        public bool formula_test_2()
        {
            string formula = "tall(john)";

            IFormula generatedFormula = new FormulaUtils().Parse(formula);
            Formula expectedFormula = new AtomicFormula("tall", new LiteralTerm("john"));

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(ExpectedResult = true)]
        [Test]
        public bool formula_test_3()
        {
            string formula = "tall(john)";

            IFormula generatedFormula = new FormulaUtils().Parse(formula);
            Formula expectedFormula = new AtomicFormula("tall", new LiteralTerm("john"), new LiteralTerm("185"));

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(ExpectedResult = true)]
        [Test]
        public bool formula_test_4()
        {
            string formula = "f(x<-int(2))";

            IFormula generatedFormula = new FormulaUtils().Parse(formula);
            Formula expectedFormula = new AtomicFormula("f", new ValuedTerm<int>(2));

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(ExpectedResult = true)]
        [Test]
        public bool formula_test_5()
        {
            string formula = "f(x<-float(2.9))";
            
            IFormula generatedFormula = new FormulaUtils().Parse(formula);
            Formula expectedFormula = new AtomicFormula("f", new ValuedTerm<float>(2.9f));

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(ExpectedResult = true)]
        [Test]
        public bool formula_test_6()
        {
            string formula = "f(x<-byte(128))";

            IFormula generatedFormula = new FormulaUtils().Parse(formula);
            Formula expectedFormula = new AtomicFormula("f", new ValuedTerm<byte>(128));

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(ExpectedResult = true)]
        [Test]
        public bool formula_test_7()
        {
            string formula = "f(x<-double(128.3282732),l,k)";

            IFormula generatedFormula = new FormulaUtils().Parse(formula);
            Formula expectedFormula = new AtomicFormula("f", new ValuedTerm<double>(128.3282732), new LiteralTerm("l"), new LiteralTerm("k"));

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(ExpectedResult = true)]
        public bool formula_test_8()
        {
            string formula = "f(x<-string(\"hi\"))";

            IFormula generatedFormula = new FormulaUtils().Parse(formula);
            Formula expectedFormula = new AtomicFormula("f", new ValuedTerm<string>("hi"));

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(ExpectedResult = true)]
        [Test]
        public bool formula_test_9()
        {
            string formula = "f(x<-char(\'c\'))";

            
            IFormula generatedFormula = new FormulaUtils().Parse(formula);
            Formula expectedFormula = new AtomicFormula("f", new ValuedTerm<char>('c'));

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(ExpectedResult = true)]
        [Test]
        public bool formula_test_10()
        {
            string formula = "f(x<-decimal(99.9m))";

            IFormula generatedFormula = new FormulaUtils().Parse(formula);
            Formula expectedFormula = new AtomicFormula("f", new ValuedTerm<decimal>(99.9m));

            return generatedFormula.Equals(expectedFormula);
        }


        [TestCase(ExpectedResult = true)]
        [Test]
        public bool formula_test_11()
        {
            string formula = "f(x<-string(\"hi mate\"))";

            IFormula generatedFormula = new FormulaUtils().Parse(formula);
            Formula expectedFormula = new AtomicFormula("f", new ValuedTerm<string>("hi mate"));

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(ExpectedResult = true)]
        [Test]
        public bool formula_test_12()
        {
            string formula = "f(k,x<-bool(\"true\"))";

            IFormula generatedFormula = new FormulaUtils().Parse(formula);
            Formula expectedFormula = new AtomicFormula("f", new LiteralTerm("k"), new ValuedTerm<bool>(true));

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(ExpectedResult = true)]
        [Test]
        public bool formula_test_13()
        {
            string formula = "f(k,x<-bool(\"false\"),p)";

            IFormula generatedFormula = new FormulaUtils().Parse(formula);
            Formula expectedFormula = new AtomicFormula("f", new LiteralTerm("k"), new ValuedTerm<bool>(false), new LiteralTerm("p"));

            return generatedFormula.Equals(expectedFormula);
        }

    }
}
