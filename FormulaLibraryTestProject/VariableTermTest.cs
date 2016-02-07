//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  VariableTermTest.cs
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

using NUnit.Framework;
using FormulaLibrary;

namespace FormulaLibraryTestProject
{
    [TestFixture]
    class VariableTermTest
    {
        [Test]
        public void toLiteralTermTest()
        {
            VariableTerm<int> var_a = new VariableTerm<int>("var", 5);
            LiteralTerm literal_a = var_a.toLiteralTerm();

            Assert.That(string.IsNullOrEmpty(literal_a.Name), Is.False, "Converted literal term has not been assigned a name.");
            Assert.IsInstanceOf<LiteralTerm>(literal_a, "Variable to literal term conversion has failed.");
        }

        /// <summary>
        /// Create a variable term using a basic simple type (short)
        /// </summary>
        [Test]
        public void createSimpleTypeVariableTerm()
        {
            VariableTerm<short> var = new VariableTerm<short>("var", 5);

            Assert.IsInstanceOf<Term>(var, "Variable term [var] is not a valid Term object.");
            Assert.That(string.IsNullOrEmpty(var.Name), Is.False, "Variable term's name is null.");
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
            Assert.That(string.IsNullOrEmpty(var.Name), Is.False, "Variable term's name is null.");
            Assert.IsNotNull(var.Value, "Variable term's value is null.");
        }

        [TestCase("var_a", 1, "var_b", 1, ExpectedResult = false)]
        [TestCase("var_a", 1, "var_a", 1, ExpectedResult = true)]
        [TestCase("var_a", 1, "var_a", 2, ExpectedResult = false)]
        [Test]
        public bool variableTermsEqualsTest(string var_a_name, short var_a_value, string var_b_name, short var_b_value)
        {
            VariableTerm<short> var_a = new VariableTerm<short>(var_a_name, var_a_value);
            VariableTerm<short> var_b = new VariableTerm<short>(var_b_name, var_b_value);
            
            return var_a.Equals(var_b);
        }


        [TestCase("", 1, "", 1, ExpectedResult = true)]
        [TestCase("var", 1, "var_b", 2, ExpectedResult = false)]
        [TestCase("var", 1, "var", 2, ExpectedResult = true)]
        [TestCase("var", 1, "var", 1, ExpectedResult = true)]
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
