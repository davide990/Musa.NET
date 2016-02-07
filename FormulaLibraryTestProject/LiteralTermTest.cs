//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  LiteralTermTest.cs
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
    public class LiteralTermTest
    {
        [Test]
        public void toVariableStringTerm()
        {
            LiteralTerm a = new LiteralTerm("dummy");
            VariableTerm<string> var = a.toVariableTerm("dummy_value");

            Assert.That(string.IsNullOrEmpty(var.Value), Is.False, "converted variable term has been not assigned to a value.");
            Assert.IsInstanceOf<VariableTerm<string>>(var, "variable term has not been converted correctly to VariableTerm<..>");
        }

        [Test]
        public void createSampleLiteralTerm()
        {
            LiteralTerm a = new LiteralTerm("dummy");

            Assert.IsNotNull(a, "Test Literal term constructor has failed its execution");
            Assert.That(string.IsNullOrEmpty(a.Name), Is.False, "Literal term has no name associated");
        }

        [TestCase("a", "a", ExpectedResult = true)]
        [TestCase("a", "b", ExpectedResult = false)]
        [Test]
        public bool literalTermsEqualsTest(string term_a_name, string term_b_name)
        {
            LiteralTerm a = new LiteralTerm(term_a_name);
            LiteralTerm b = new LiteralTerm(term_b_name);

            return a.Equals(b);

        }
    }
}
