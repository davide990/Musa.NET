using FormulaLibrary;
using FormulaLibrary.ANTLR;
using NUnit.Framework;

namespace FormulaLibraryTest.ANTLRTest
{
    [TestFixture]
    class SimpleFormulaParserTest
    {
        [TestCase(Result = true)]
        [Test]
        public bool formula_test_0()
        {
            string formula = "f(x)";

            Formula generatedFormula = FormulaParser.ConvertStringToFormula(formula);
            Formula expectedFormula = new AtomicFormula("f", new LiteralTerm("x"));

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(Result = true)]
        [Test]
        public bool formula_test_1()
        {
            string formula = "f(x,y,k)";

            Formula generatedFormula = FormulaParser.ConvertStringToFormula(formula);
            Formula expectedFormula = new AtomicFormula("f", new LiteralTerm("x"), new LiteralTerm("y"), new LiteralTerm("k"));

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(Result = true)]
        [Test]
        public bool formula_test_2()
        {
            string formula = "tall(john)";

            Formula generatedFormula = FormulaParser.ConvertStringToFormula(formula);
            Formula expectedFormula = new AtomicFormula("tall", new LiteralTerm("john"));

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(Result = true)]
        [Test]
        public bool formula_test_3()
        {
            string formula = "tall(john)";

            Formula generatedFormula = FormulaParser.ConvertStringToFormula(formula);
            Formula expectedFormula = new AtomicFormula("tall", new LiteralTerm("john"), new LiteralTerm("185"));

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(Result = true)]
        [Test]
        public bool formula_test_4()
        {
            string formula = "f(x<-int(2))";

            Formula generatedFormula = FormulaParser.ConvertStringToFormula(formula);
            Formula expectedFormula = new AtomicFormula("f", new VariableTerm<int>("x",2));

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(Result = true)]
        [Test]
        public bool formula_test_5()
        {
            string formula = "f(x<-float(2.9))";
            
            Formula generatedFormula = FormulaParser.ConvertStringToFormula(formula);
            Formula expectedFormula = new AtomicFormula("f", new VariableTerm<float>("x", 2.9f));

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(Result = true)]
        [Test]
        public bool formula_test_6()
        {
            string formula = "f(x<-byte(128))";

            Formula generatedFormula = FormulaParser.ConvertStringToFormula(formula);
            Formula expectedFormula = new AtomicFormula("f", new VariableTerm<byte>("x", 128));

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(Result = true)]
        [Test]
        public bool formula_test_7()
        {
            string formula = "f(x<-double(128.3282732),l,k)";

            Formula generatedFormula = FormulaParser.ConvertStringToFormula(formula);
            Formula expectedFormula = new AtomicFormula("f", new VariableTerm<double>("x", 128.3282732), new LiteralTerm("l"), new LiteralTerm("k"));

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(Result = true)]
        public bool formula_test_8()
        {
            string formula = "f(x<-string(\"hi\"))";

            Formula generatedFormula = FormulaParser.ConvertStringToFormula(formula);
            Formula expectedFormula = new AtomicFormula("f", new VariableTerm<string>("x", "hi"));

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(Result = true)]
        [Test]
        public bool formula_test_9()
        {
            string formula = "f(x<-char(\'c\'))";

            
            Formula generatedFormula = FormulaParser.ConvertStringToFormula(formula);
            Formula expectedFormula = new AtomicFormula("f", new VariableTerm<char>("x", 'c'));

            return generatedFormula.Equals(expectedFormula);
        }

        [TestCase(Result = true)]
        [Test]
        public bool formula_test_10()
        {
            string formula = "f(x<-decimal(99.9m))";

            Formula generatedFormula = FormulaParser.ConvertStringToFormula(formula);
            Formula expectedFormula = new AtomicFormula("f", new VariableTerm<decimal>("x", 99.9m));

            return generatedFormula.Equals(expectedFormula);
        }


        [TestCase(Result = true)]
        [Test]
        public bool formula_test_11()
        {
            string formula = "f(x<-string(\"hi mate\"))";

            Formula generatedFormula = FormulaParser.ConvertStringToFormula(formula);
            Formula expectedFormula = new AtomicFormula("f", new VariableTerm<string>("x", "hi mate"));

            return generatedFormula.Equals(expectedFormula);
        }

    }
}
