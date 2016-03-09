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
        /// <summary>
        /// Create a variable term using a basic simple type (short)
        /// </summary>
        [Test]
        public void createSimpleTypeVariableTerm()
        {
            ValuedTerm<short> var = new ValuedTerm<short>(5);

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
            ValuedTerm<LiteralTerm> var = new ValuedTerm<LiteralTerm>(new LiteralTerm("hello"));

            Assert.IsInstanceOf<Term>(var, "Variable term [var] is not a valid Term object.");
            Assert.That(string.IsNullOrEmpty(var.Name), Is.False, "Variable term's name is null.");
            Assert.IsNotNull(var.Value, "Variable term's value is null.");
        }

        [TestCase(1,1, ExpectedResult = false)]
        [TestCase(1,1, ExpectedResult = true)]
        [TestCase(1,2, ExpectedResult = false)]
        [Test]
        public bool variableTermsEqualsTest(short var_a_value, short var_b_value)
        {
            ValuedTerm<short> var_a = new ValuedTerm<short>(var_a_value);
            ValuedTerm<short> var_b = new ValuedTerm<short>(var_b_value);
            
            return var_a.Equals(var_b);
        }
    }
}
