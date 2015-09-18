/**
         __  __                                     _   
        |  \/  |                                   | |  
        | \  / | _   _  ___   __ _     _ __    ___ | |_ 
        | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
        | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
        |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|

*/
using NUnit.Framework;
using FormulaLibrary;

namespace FormulaLibraryTest
{
    [TestFixture]
    public class LiteralTermTest
    {
        [Test]
        public void toVariableStringTerm()
        {
            LiteralTerm a = new LiteralTerm("dummy");
            VariableTerm<string> var = a.toVariableTerm("dummy_value");

            Assert.IsNotNullOrEmpty(var.Value, "converted variable term has been not assigned any value.");
            Assert.IsInstanceOf<VariableTerm<string>>(var, "variable term has not been converted correctly to VariableTerm<..>");
        }

        [Test]
        public void createSampleLiteralTerm()
        {
            LiteralTerm a = new LiteralTerm("dummy");

            Assert.IsNotNull(a, "Test Literal term constructor has failed its execution");
            Assert.IsNotNullOrEmpty(a.Name, "Literal term has no name associated");
        }

        [TestCase("a", "a", Result = true)]
        [TestCase("a", "b", Result = false)]
        [Test]
        public bool literalTermsEqualsTest(string term_a_name, string term_b_name)
        {
            LiteralTerm a = new LiteralTerm(term_a_name);
            LiteralTerm b = new LiteralTerm(term_b_name);

            return a.Equals(b);

        }
    }
}
