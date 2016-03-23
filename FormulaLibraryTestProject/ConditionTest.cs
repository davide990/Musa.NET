//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  ConditionTest.cs
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
using AgentLibrary;
using FormulaLibrary;
using MusaCommon;
using System.Collections.Generic;

namespace FormulaLibraryTestProject
{
    [TestFixture]
    public class ConditionTest
    {
        [TestCase("f(x)", ExpectedResult = true)]
        [TestCase("f(x,y)", ExpectedResult = true)]
        [TestCase("f(x,y)&g(p)", ExpectedResult = true)]
        [TestCase("f(x)&g(p)", ExpectedResult = true)]
        [TestCase("f(x,y)&g(k,l)", ExpectedResult = false)]
        [TestCase("f(8)", ExpectedResult = true)]
        [TestCase("f(8,y)", ExpectedResult = true)]
        [TestCase("f(x,8999)|g(1)", ExpectedResult = true)]
        [Test]
        public bool test1(string Formula)
        {
            MusaInitializer.MUSAInitializer.Initialize();
            AgentWorkbench wb = new AgentWorkbench(new Agent());
            var parser = new FormulaUtils();
            wb.AddStatement(parser.Parse("f(x)"), parser.Parse("f(x,y)"), parser.Parse("g(p)"));

            Console.WriteLine("For formula " + Formula + " generated assignment: ");
            List<IAssignment> assignment;
            var satisfied = wb.TestCondition(parser.Parse(Formula), out assignment);

            return satisfied;
        }


        [TestCase("f(x)", ExpectedResult = true)]
        [TestCase("f(x,y)", ExpectedResult = true)]
        [TestCase("f(x,y)&g(p)", ExpectedResult = true)]
        [TestCase("f(x)&g(p)", ExpectedResult = true)]
        [TestCase("f(x,y)&g(k,l)", ExpectedResult = false)]
        [Test]
        public bool test2(string Formula)
        {
            MusaInitializer.MUSAInitializer.Initialize();
            AgentWorkbench wb = new AgentWorkbench(new Agent());
            var parser = new FormulaUtils();
            wb.AddStatement(parser.Parse("f(\"ciao\")"), parser.Parse("f(x,y)"), parser.Parse("g(p)"));

            Console.WriteLine("For formula " + Formula + " generated assignment: ");
            List<IAssignment> assignment;
            var satisfied = wb.TestCondition(parser.Parse(Formula), out assignment);

            return satisfied;
        }


		[TestCase("!f(x)", ExpectedResult = false)]
		[TestCase("!p(x)", ExpectedResult = true)]
		[TestCase("f(x)&!g(p)", ExpectedResult = false)]
		[TestCase("f(x)|!g(p)", ExpectedResult = true)]
		[TestCase("f(x)&g(p)", ExpectedResult = true)]
		[TestCase("!f(x)&g(3)&(f(x)&!g(x))", ExpectedResult = false)]
		[TestCase("!f(x)|g(3)&(k(3)|!g(x))", ExpectedResult = true)]
		[Test]
		public bool test3(string Formula)
		{
			MusaInitializer.MUSAInitializer.Initialize();
			AgentWorkbench wb = new AgentWorkbench(new Agent());
			var parser = new FormulaUtils();
			wb.AddStatement(parser.Parse("f(\"ciao\")"), parser.Parse("k(x)"), parser.Parse("g(p)"));

			Console.WriteLine("For formula " + Formula + " generated assignment: ");
			List<IAssignment> assignment;
			var satisfied = wb.TestCondition(parser.Parse(Formula), out assignment);

			return satisfied;
		}

    }
}

