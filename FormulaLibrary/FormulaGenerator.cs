//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  FormulaGenerator.cs
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
using System.Collections.Generic;
using System.Linq;

namespace FormulaLibrary
{
    /// <summary>
    /// Utility class for generating random formulas
    /// </summary>
    public class FormulaGenerator
    {
        private readonly string[] SupportedDataTypes = { "System.Boolean", "System.Int32", "System.Single", "System.String" };

        private readonly Random rand;

        /// <summary>
        /// The maximum number of terms that each formula could contains
        /// </summary>
        private int maxTerms;
        
        /// <summary>
        /// The maximum lenght of the functor for each formula
        /// </summary>
        private int functorMaxLenght;
        
        /// <summary>
        /// Specify if the generated formula can be or not parametric
        /// </summary>
        private bool parametric;
        
        /// <summary>
        /// The maximum depth of the hiearchy tree of formula
        /// </summary>
        private int maxDepth;

        /// <summary>
        /// The maximum length for terms
        /// </summary>
        private int maxTermLenght;

        private int SupportedDataTypesNum
        {
            get { return SupportedDataTypes.Length; }
        }


        public FormulaGenerator(int MaxDepth, int MaxNumTerms, int MaxTermLength, int FunctorMaxLenght, bool Parametric)
        {
            rand = new Random();
            maxTerms = MaxNumTerms;
            functorMaxLenght = FunctorMaxLenght;
            parametric = Parametric;
            maxTermLenght = MaxTermLength;
            maxDepth = MaxDepth;

            //Ensure that the decimal numbers separator is the dot instead of comma
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
        }

        /// <summary>
        /// Generate a random generic formula that may contains and, or, not logic predicates
        /// </summary>
        public Formula GetRandomFormula()
        {
            return GetRandomFormula(0);
        }

        /// <summary>
        /// Generate a random generic formula that may contains and, or, not logic predicates
        /// </summary>
        private Formula GetRandomFormula(int depth)
        {
            if (depth > maxDepth)
                return GetRandomAtomicFormula();

            switch (rand.Next(0, 3))
            {
                case 0:
                    return new AndFormula(GetRandomFormula(depth + 1), GetRandomFormula(depth + 1));
                case 1:
                    return new OrFormula(GetRandomFormula(depth + 1), GetRandomFormula(depth + 1));
                case 2:
                    return new NotFormula(GetRandomFormula(depth + 1));
                case 3:
                default:
                    return GetRandomAtomicFormula();
            }
        }

        /// <summary>
        /// Generate a random atomic formula
        /// </summary>
        public AtomicFormula GetRandomAtomicFormula()
        {
            string functor = RandomString(functorMaxLenght);
            List<Term> terms = new List<Term>();

            int numTerms = rand.Next(1, maxTerms);

            for (int i = 0; i < numTerms; i++)
            {
                //If formula must not contains variable terms, just add a new literal term
                if (!parametric)
                {
                    terms.Add(new LiteralTerm(RandomString(rand.Next(1, maxTermLenght))));
                    continue;
                }

                //If a random number is greater or equal to 0.5, create a new variable term
                if (rand.NextDouble() >= 0.5)
                    terms.Add(GenerateRandomValuedTerm());
                else
                    //otherwise, add a new literal term
                    terms.Add(new LiteralTerm(RandomString(rand.Next(1, maxTermLenght))));
            }

            return new AtomicFormula(functor, terms);
        }


        /// <summary>
        /// Generate a random variable term
        /// </summary>
        /// <returns></returns>
        private Term GenerateRandomValuedTerm()
        {
            switch (SupportedDataTypes[rand.Next(0, SupportedDataTypesNum)])
            {
                case "System.String":
                    return new ValuedTerm<string>(RandomString(rand.Next(1, maxTermLenght)));
                case "System.Boolean":
                    return new ValuedTerm<bool>(GenerateRandomBool());
                case "System.Single":
                    return new ValuedTerm<float>(Convert.ToSingle(rand.NextDouble()));
                case "System.Int32":
                default:
                    return new ValuedTerm<int>(rand.Next());
            }
        }

        /// <summary>
        /// Generate a random bool value
        /// </summary>
        private bool GenerateRandomBool()
        {
            if (rand.NextDouble() >= 0.5)
                return true;
            return false;
        }

        /// <summary>
        /// Generate a random string
        /// </summary>
        /// <param name="length">the lenght of the random generated string</param>
        private string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[rand.Next(1, s.Length)]).ToArray());
        }
    }
}
