//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  AtomicFormulaTest.cs
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

namespace FormulaLibraryTestProject
{
    [TestFixture]
    class AtomicFormulaTest
    {
        [Test]
        public void atomicFormulaFromString()
        {
        }

        [TestCase(false, ExpectedResult = "f(x,y<-short(1))")]
        [TestCase(true, ExpectedResult = "f(x,y)")]
        [Test]
        public string atomicFormulaToString(bool compactVisualization)
        {
            AtomicFormula a = new AtomicFormula("f", new LiteralTerm("x"), new VariableTerm<short>("y", 1));
            return a.ToString(compactVisualization);
        }

        [TestCase(ExpectedResult = true)]
        [Test]
        public bool checkAtomicFormulaEquals_0()
        {
            AtomicFormula formula_a = new AtomicFormula("f", new Term[] { new LiteralTerm("x") });
            AtomicFormula formula_b = new AtomicFormula("f", new Term[] { new LiteralTerm("x") });
            
            return formula_a.Equals(formula_b);
        }

        [TestCase(ExpectedResult = false)]
        [Test]
        public bool checkAtomicFormulaEquals_1()
        {
            AtomicFormula formula_a = new AtomicFormula("f", new Term[] { new LiteralTerm("x") });
            AtomicFormula formula_b = new AtomicFormula("g", new Term[] { new LiteralTerm("x") });

            return formula_a.Equals(formula_b);
        }

        [TestCase(ExpectedResult = false)]
        [Test]
        public bool checkAtomicFormulaEquals_2()
        {
            AtomicFormula formula_a = new AtomicFormula("f", new Term[] { new LiteralTerm("x") });
            AtomicFormula formula_b = new AtomicFormula("f", new Term[] { new LiteralTerm("y") });

            return formula_a.Equals(formula_b);
        }

        [TestCase(ExpectedResult = false)]
        [Test]
        public bool checkAtomicFormulaEquals_3()
        {
            AtomicFormula formula_a = new AtomicFormula("f", new Term[] { new VariableTerm<int>("x",1) });
            AtomicFormula formula_b = new AtomicFormula("f", new Term[] { new VariableTerm<string>("x", "hello") });

            return formula_a.Equals(formula_b);
        }

        [TestCase(ExpectedResult = false)]
        [Test]
        public bool checkAtomicFormulaEquals_4()
        {
            AtomicFormula formula_a = new AtomicFormula("f", new Term[] { new VariableTerm<int>("x", 1) });
            AtomicFormula formula_b = new AtomicFormula("g", new Term[] { new VariableTerm<int>("x", 1) });

            return formula_a.Equals(formula_b);
        }

    }
}
