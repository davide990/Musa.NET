/**
         __  __                                     _   
        |  \/  |                                   | |  
        | \  / | _   _  ___   __ _     _ __    ___ | |_ 
        | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
        | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
        |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|

*/
using System;
using System.Collections.Generic;
using System.Text;

namespace FormulaLibrary
{
    /// <summary>
    /// A first-order logic predicate. It includes:
    /// 
    /// - a functor, and
    /// - a list of terms
    /// 
    /// </summary>
    public sealed class AtomicFormula : Formula
    {
        /// <summary>
        /// The functor of this formula
        /// </summary>
        public string Functor
        {
            get { return functor; }
        }
        private readonly string functor;

        /// <summary>
        /// The list of terms of this formula
        /// </summary>
        private List<Term> terms;

        public AtomicFormula(string functor, params Term[] terms)
        {
            this.functor = functor;
            this.terms = new List<Term>(terms);
        }

        public override FormulaType getType()
        {
            return FormulaType.ATOMIC_FORMULA;
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.Append(functor);
            b.Append("(");
            for(byte i=0;i<terms.Count;i++)
            {
                b.Append(terms[i]);
                if (i != terms.Count - 1)
                    b.Append(",");
            }
            
            b.Append(")");
            return b.ToString();
        }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}