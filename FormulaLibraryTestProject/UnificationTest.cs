//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  UnificationTest.cs
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
using System;
using NUnit.Framework;
using MusaCommon;
using System.Collections.Generic;

namespace FormulaLibraryTestProject
{
    [TestFixture]
    public class UnificationTest
    {
        //TODO implementami

        [TestCase("f(x)", ExpectedResult = true)]
        [TestCase("f(y)", ExpectedResult = true)]
        [TestCase("f(x,1)", ExpectedResult = true)]
        [TestCase("f(x,1) & k(h)", ExpectedResult = true)]
        [TestCase("k(x) & x(x)", ExpectedResult = true)]
        [TestCase("f(y) & l(i) | f(o)", ExpectedResult = true)]
        [TestCase("!r(x)", ExpectedResult = true)]
        [TestCase("r(x) | e(3,x,4)", ExpectedResult = true)]
        [TestCase("f(x,3)&g(x) | k(k) & !l(3,\"ciao\",o,x,p)", ExpectedResult = true)]
        [Test]
        public bool unification_test(string formula)
        {
            MusaInitializer.MusaInitializer.Initialize();
            var fp = ModuleProvider.Get().Resolve<IFormulaUtils>();
            var ass_fact = ModuleProvider.Get().Resolve<IAssignmentFactory>();
            IFormula ff = fp.Parse(formula);
            var assignments = new List<IAssignment>() { ass_fact.CreateAssignment("x", 3), ass_fact.CreateAssignment("o", "davide") };
            try
            {
                ff.Unify(assignments);
                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}

