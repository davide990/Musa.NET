/**
         __  __                                     _   
        |  \/  |                                   | |  
        | \  / | _   _  ___   __ _     _ __    ___ | |_ 
        | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
        | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
        |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|

*/
using FormulaLibrary;
using NUnit.Framework;
using System.Collections.Generic;

namespace FormulaLibraryTest
{
    [TestFixture]
    class AtomicFormulaTest
    {
        [Test]
        public void atomicFormulaFromString()
        {
        }

        [TestCase(false, Result = "f(x,[y,1])")]
        [TestCase(true, Result = "f(x,y)")]
        [Test]
        public string atomicFormulaToString(bool compactVisualization)
        {
            AtomicFormula a = new AtomicFormula("f", new LiteralTerm("x"), new VariableTerm<short>("y", 1));
            return a.ToString(compactVisualization);
        }

        [TestCase(Result = true)]
        [Test]
        public bool checkAtomicFormulaEquals_0()
        {
            AtomicFormula formula_a = new AtomicFormula("f", new Term[] { new LiteralTerm("x") });
            AtomicFormula formula_b = new AtomicFormula("f", new Term[] { new LiteralTerm("x") });
            
            return formula_a.Equals(formula_b);
        }

        [TestCase(Result = false)]
        [Test]
        public bool checkAtomicFormulaEquals_1()
        {
            AtomicFormula formula_a = new AtomicFormula("f", new Term[] { new LiteralTerm("x") });
            AtomicFormula formula_b = new AtomicFormula("g", new Term[] { new LiteralTerm("x") });

            return formula_a.Equals(formula_b);
        }

        [TestCase(Result = false)]
        [Test]
        public bool checkAtomicFormulaEquals_2()
        {
            AtomicFormula formula_a = new AtomicFormula("f", new Term[] { new LiteralTerm("x") });
            AtomicFormula formula_b = new AtomicFormula("f", new Term[] { new LiteralTerm("y") });

            return formula_a.Equals(formula_b);
        }

        [TestCase(Result = false)]
        [Test]
        public bool checkAtomicFormulaEquals_3()
        {
            AtomicFormula formula_a = new AtomicFormula("f", new Term[] { new VariableTerm<int>("x",1) });
            AtomicFormula formula_b = new AtomicFormula("f", new Term[] { new VariableTerm<string>("x", "hello") });

            return formula_a.Equals(formula_b);
        }

        [TestCase(Result = false)]
        [Test]
        public bool checkAtomicFormulaEquals_4()
        {
            AtomicFormula formula_a = new AtomicFormula("f", new Term[] { new VariableTerm<int>("x", 1) });
            AtomicFormula formula_b = new AtomicFormula("g", new Term[] { new VariableTerm<int>("x", 1) });

            return formula_a.Equals(formula_b);
        }

    }
}
