using AgentLibrary;
using FormulaLibrary.ANTLR;
using FormulaLibrary;
using NUnit.Framework;

namespace AgentLibraryTest
{
    [TestFixture]
    public class AgentWorkbenchTest
    {
        [TestCase("y(x <-int(3))", Result = true)]
        [TestCase("k(x)", Result = false)]
        [TestCase("k(x,y)", Result = false)]
        [TestCase("y(x,y)", Result = false)]
        [TestCase("y(x)", Result = true)]
        [TestCase("o(l<-int(2),k)", Result = true)]
        [TestCase("h(s,o,a)", Result = true)]
        [Test]
        public bool test_simple(string formula)
        {
            AgentWorkbench wb = new AgentWorkbench();
            wb.addStatement(FormulaParser.Parse("y(x<-int(3))") as AtomicFormula,
                            FormulaParser.Parse("f(x)") as AtomicFormula,
                            FormulaParser.Parse("h(s,o,a<-string(\"ciao mondo\"))") as AtomicFormula,
                            FormulaParser.Parse("o(m,s<-char('d')") as AtomicFormula);

            AtomicFormula test = FormulaParser.Parse(formula) as AtomicFormula;
            
            return wb.TestCondition(test);
        }

        [TestCase("y(x)&f(x)", Result = true)]
        [TestCase("f(x)&o(m,k)", Result = true)]
        [TestCase("f(x)&o(m)", Result = false)]
        [TestCase("f(x)|o(m)", Result = true)]
        [TestCase("!f(x,u,a)", Result = true)]
        [TestCase("!f(x,u,a)&(y(l)|!(l(q,w,e,r,t,y)))", Result = true)]
        [Test]
        public bool test_composite(string formula)
        {
            AgentWorkbench wb = new AgentWorkbench();
            wb.addStatement(FormulaParser.Parse("y(x<-int(3))") as AtomicFormula,
                            FormulaParser.Parse("f(x)") as AtomicFormula,
                            FormulaParser.Parse("h(s,o,a<-string(\"ciao mondo\"))") as AtomicFormula,
                            FormulaParser.Parse("o(m,s<-char('d')") as AtomicFormula);

            Formula test = FormulaParser.Parse(formula);

            return wb.TestCondition(test);
        }
    }
}
