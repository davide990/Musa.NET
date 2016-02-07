//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  AtomicFormula.cs
//
//  Author:
//       Davide Guastella <davide.guastella90@gmail.com>
//
//  Copyright (c) 2015 Davide Guastella
//
//  This program is free software: you can redistribute it and/or modify
//  it under the Terms of the GNU Lesser General Public License as published by
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
using System.Reflection;
using System.Text;

namespace FormulaLibrary
{
    /// <summary>
    /// A first-order logic predicate. It includes:
    /// 
    /// - a functor, and
    /// - a list of Terms
    /// </summary>
    public sealed class AtomicFormula : Formula, ICloneable
    {
        /// <summary>
        /// The functor of this formula
        /// </summary>
        public string Functor
        {
            get;
            private set;
        }

        /// <summary>
        /// The list of Terms of this formula
        /// </summary>
        public List<Term> Terms
        {
            get;
            private set;
        }

        /// <summary>
        /// Return the number of terms of this formula
        /// </summary>
        public int TermsCount
        {
            get { return Terms.Count; }
        }

        public AtomicFormula(string functor, params Term[] Terms)
        {
            Functor = functor;
            this.Terms = new List<Term>(Terms);
        }

        public AtomicFormula(string functor, IEnumerable<Term> Terms)
        {
            Functor = functor;
            this.Terms = new List<Term>(Terms);
            //Terms.AddRange(Terms);
        }


        public override FormulaType getType()
        {
            return FormulaType.ATOMIC_FORMULA;
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.Append(Functor);
            b.Append("(");
            for (byte i = 0; i < Terms.Count; i++)
            {
                b.Append(Terms[i]);
                if (i != Terms.Count - 1)
                    b.Append(",");
            }
            
            b.Append(")");
            return b.ToString();
        }

        /// <summary>
        /// Return a string representation for this formula, i.e.
        ///
        /// f(x,y,z)
        /// 
        /// If compactVisualization is true, then each term's name is printed. Otherwise,
        /// for each variable term is printed also its value.
        /// </summary>
        public string ToString(bool compactVisualization)
        {
            if (!compactVisualization)
                return ToString();

            StringBuilder b = new StringBuilder();
            b.Append(Functor);
            b.Append("(");
            for (byte i = 0; i < Terms.Count; i++)
            {
                b.Append(Terms[i].Name);
                if (i != Terms.Count - 1)
                    b.Append(",");
            }

            b.Append(")");
            return b.ToString();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is AtomicFormula))
                return false;

            AtomicFormula other = (AtomicFormula)obj;

            if (!other.Functor.Equals(Functor))
                return false;
            
            foreach (var t in Terms)
            {
                if (!other.Terms.Contains(t))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Check if this formula is simple or parametric (that is, check if
        /// this formula contains or not variable Terms)
        /// </summary>
        public override bool IsParametric()
        {
            foreach (Term t in Terms)
            {
                if (!(t is LiteralTerm))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Convert this formula to a simple type formula. That is, if this formula
        /// contains any variable term, convert them to literal Terms. Found variable
        /// Terms are returned to the output list
        /// </summary>
        /// <returns>a list containing the variable Terms this formula previously contained</returns>
        public List<object> ConvertToSimpleFormula()
        {
            List<object> variableTerms = new List<object>();
            
            //Iterate each term
            for (int i = 0; i < Terms.Count; i++)
            {
                //if a variable term occurs, convert each one to literal term
                if (Terms[i].GetType().IsGenericType)
                {
                    //add the variable term to the output list
                    variableTerms.Add(Terms[i]);

                    //get the type info for the current term
                    Type variableTermType = VariableTermFacace.GetVariableTermFor(Terms[i].GetType().GetGenericArguments()[0]);
                    Terms[i] = VariableTermFacace.ConvertToLiteralTerm(Terms[i]);
                }
            }

            return variableTerms;
        }

        /// <summary>
        /// Create a clone of this formula
        /// </summary>
        public object Clone()
        {
            AtomicFormula clone = new AtomicFormula(Functor);

            //Iterate each term
            for (int i = 0; i < Terms.Count; i++)
            {
                //if a variable term occurs
                if (Terms[i].GetType().IsGenericType)
                { 
                    //get the type info for the current term
                    Type variableTermType = VariableTermFacace.GetVariableTermFor(Terms[i].GetType().GetGenericArguments()[0]);

                    string name = (string)VariableTermFacace.GetNameOfVariableTerm(Terms[i]);
                    object value = VariableTermFacace.GetValueOfVariableTerm(Terms[i]);
                    object varTerm = VariableTermFacace.CreateVariableTerm(name, value);

                    //add the new instance to the cloned formula
                    clone.Terms.Add((Term)varTerm);
                }
                else
                {
                    clone.Terms.Add(new LiteralTerm(Terms[i].Name));
                }
            }
            return clone;
        }
    }

    sealed internal class InvalidFormulaFormatException : Exception
    {
        public InvalidFormulaFormatException()
        {
        }

        public InvalidFormulaFormatException(string message)
            : base(message)
        {
        }

        public InvalidFormulaFormatException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}