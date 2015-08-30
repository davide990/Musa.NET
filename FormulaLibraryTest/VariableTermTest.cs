using NUnit.Framework;
using FormulaLibrary;

namespace FormulaLibraryTest
{
    [TestFixture]
    class VariableTermTest
    {
        [Test]
        public void toLiteralTermTest()
        {
            VariableTerm<int> var_a = new VariableTerm<int>("var",5);
            LiteralTerm literal_a = var_a.toLiteralTerm();

            Assert.IsInstanceOf<LiteralTerm>(literal_a, "Variable to literal term conversion has failed.");
            Assert.IsNotNullOrEmpty(literal_a.Name, "Converted literal term has not been assigned a name.");
        }

        /// <summary>
        /// Create a variable term using a basic simple type (short)
        /// </summary>
        [Test]
        public void createSimpleTypeVariableTerm()
        {
            VariableTerm<short> var = new VariableTerm<short>("var", 5);

            Assert.IsInstanceOf<Term>(var, "Variable term [var] is not a valid Term object.");
            Assert.IsNotNullOrEmpty(var.Name, "Variable term's name is null.");
            Assert.IsNotNull(var.Value, "Variable term's value is null.");
        }

        /// <summary>
        /// Create a variable term using LiteralTerm type as variable type.
        /// </summary>
        [Test]
        public void createGenericTypeVariableTerm()
        {
            VariableTerm<LiteralTerm> var = new VariableTerm<LiteralTerm>("var", new LiteralTerm("hello"));

            Assert.IsInstanceOf<Term>(var, "Variable term [var] is not a valid Term object.");
            Assert.IsNotNullOrEmpty(var.Name, "Variable term's name is null.");
            Assert.IsNotNull(var.Value, "Variable term's value is null.");
        }

        [TestCase("var_a", 1, "var_b", 1, Result = false)]
        [TestCase("var_a", 1, "var_a", 1, Result = true)]
        [TestCase("var_a", 1, "var_a", 2, Result = false)]
        [Test]
        public bool variableTermsEqualsTest(string var_a_name, short var_a_value, string var_b_name, short var_b_value)
        {
            VariableTerm<short> var_a = new VariableTerm<short>(var_a_name, var_a_value);
            VariableTerm<short> var_b = new VariableTerm<short>(var_b_name, var_b_value);
            
            return var_a.Equals(var_b);
        }


        [TestCase("", 1, "", 1, Result = true)]
        [TestCase("var", 1, "var_b", 2, Result = false)]
        [TestCase("var", 1, "var", 2, Result = true)]
        [TestCase("var", 1, "var", 1, Result = true)]
        [Test]
        public bool unificationTest(string term_name, short initial_value, string assignment_name, short assignment_value)
        {
            VariableTerm<short> var = new VariableTerm<short>(term_name, initial_value);
            Assignment<short> a = new Assignment<short>(assignment_name, assignment_value);
            Assert.AreEqual(var.Value, initial_value, "Variable term has not been correctly assigned the input value.");
            return var.unify(a);
        }
        
    }
}
