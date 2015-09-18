using AgentLibrary;
using FormulaLibrary.ANTLR;
using FormulaLibrary;
using NUnit.Framework;

namespace AgentLibraryTest
{
    [TestFixture]
    public class AgentWorkbenchTest
    {
        [TestCase(Result = true)]
        [Test]
        public bool test_1()
        {
            AgentWorkbench wb = new AgentWorkbench();

            AtomicFormula ff = FormulaParser.Parse("f(x)") as AtomicFormula;
            wb.addCondition(ff);

            return wb.TestCondition(ff);

        }

        [TestCase(Result = true)]
        [Test]
        public bool test_2()
        {
            AgentWorkbench wb = new AgentWorkbench();
            AtomicFormula ff = FormulaParser.Parse("y(x<-int(3))") as AtomicFormula;
            wb.addCondition(ff);

            AtomicFormula test = FormulaParser.Parse("y(x<-int(3))") as AtomicFormula;


            return wb.TestCondition(test);
        }

        [TestCase(Result = true)]
        [Test]
        public bool test_3()
        {
            AgentWorkbench wb = new AgentWorkbench();
            AtomicFormula ff = FormulaParser.Parse("y(x<-int(3))") as AtomicFormula;
            wb.addCondition(ff);

            AtomicFormula test = FormulaParser.Parse("y(x)") as AtomicFormula;


            return wb.TestCondition(test);
        }

    }
}
