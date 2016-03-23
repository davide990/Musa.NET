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
using System.Text;
using MusaCommon;

namespace FormulaLibrary
{
    /// <summary>
    /// A first-order logic predicate. It includes:
    /// 
    /// - a functor, and
    /// - a list of Terms
    /// </summary>
    public sealed class AtomicFormula : Formula, IAtomicFormula//, ICloneable
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
        public List<ITerm> Terms
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

        public ValuedTermFacace ValuedTermFacace
        {
            get;
            private set;
        }

        public AtomicFormula(string functor, params ITerm[] Terms)
        {
            Functor = functor;
            this.Terms = new List<ITerm>(Terms);
            ValuedTermFacace = new ValuedTermFacace();
        }

        public AtomicFormula(string functor, IEnumerable<ITerm> Terms)
        {
            Functor = functor;
            this.Terms = new List<ITerm>(Terms);
            ValuedTermFacace = new ValuedTermFacace();
        }


        public override FormulaType GetFormulaType()
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
            int hc = 0;
            Terms.ForEach(x => hc += x.GetHashCode());
            return hc + Functor.GetHashCode();
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

        public override bool IsAtomic()
        {
            return true;
        }

        /// <summary>
        /// Create a clone of this formula
        /// </summary>
        public override object Clone()
        {
            AtomicFormula clone = new AtomicFormula(Functor);

            //Iterate each term
            for (int i = 0; i < Terms.Count; i++)
            {
                //if a variable term occurs
                if (!Terms[i].IsLiteral())
                {
                    object varTerm = ValuedTermFacace.CreateValuedTerm((Terms[i] as ITerm).GetValue());

                    //add the new instance to the cloned formula
                    clone.Terms.Add(varTerm as Term);
                }
                else
                {
                    clone.Terms.Add(new LiteralTerm(Terms[i].GetName()));
                }
            }
            return clone;
        }

        public override void Unify(List<IAssignment> assignment)
        {
            for (int i = 0; i < TermsCount; i++)
            {
                if (!Terms[i].IsLiteral())
                    continue;

                var the_assignment = assignment.Find(x => x.GetName().Equals(Terms[i].GetName()));
                if (the_assignment != null)
                    Terms[i] = (Terms[i] as LiteralTerm).Unify(the_assignment);
            }
        }


        #region IAtomicFormula members

        public string GetFunctor()
        {
            return Functor;
        }

        public ITerm[] GetTerms()
        {
            return Terms.ToArray();
        }

        public ITerm GetTermAt(int i)
        {
            return Terms[i];
        }

        public int GetTermsCount()
        {
            return Terms.Count;
        }

        #endregion IAtomicFormula members

        /// <summary>
        /// Check if this formula is assignable from a specified formula, eventually
        /// using a set of assignments.
        /// </summary>
        /// <param name="f">The formula to be tested</param>
        /// <returns>True if this formula is equals to f, eventually unifying f with the generated assignment set</returns>
        public override bool MatchWith(IFormula f, out List<IAssignment> generatedAssignment)
        {
            generatedAssignment = new List<IAssignment>();
            if (!f.IsAtomic())
                return false;

            ITerm a, b;
            bool success = true;
            var input_formula = f as IAtomicFormula;


            if (GetTermsCount() != input_formula.GetTermsCount() || !GetFunctor().Equals(input_formula.GetFunctor()))
                return false;

            generatedAssignment.Clear();
            success = true;

            //Iterate each belief's term
            for (int i = 0; i < GetTermsCount(); i++)
            {
                a = GetTermAt(i);
                b = input_formula.GetTermAt(i);

                if (!a.IsLiteral())
                {
                    if (!b.IsLiteral())
                    {
                        //a and b are both variable 
                        if (a.GetValue().Equals(b.GetValue()))
                            //if both valued terms have equal values, proceed with the next couple of terms
                            continue;
                        else
                        {
                            //terms are both valued but they have different values. The test fails here.
                            success = false;
                            break;
                        }
                    }
                    else
                    {
                        //a is variable term, b is literal. An assignment is created to assign to b the value of a
                        generatedAssignment.Add(ModuleProvider.Get().Resolve<IAssignmentFactory>().CreateAssignment(b.GetName(), a.GetValue()));
                    }
                }
                else
                {
                    if (b.IsLiteral())
                    {
                        //Here, a is leteral
                        if (a.Equals(b))
                            continue;   //b == a
                        else
                        {
                            success = false;    //b != a, test fails
                            break;
                        }
                    }
                    else
                        //b is variable, a is literal - CREA ASSIGNMENT
                        generatedAssignment.Add(ModuleProvider.Get().Resolve<IAssignmentFactory>().CreateAssignment(a.GetName(), b.GetValue()));
                }
            }
            if (success)
                return true;

            return false;
        }

    }
}