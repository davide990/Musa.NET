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
        public List<Term> Terms
        {
            get { return terms; }
        }
        private List<Term> terms;

        public AtomicFormula(string functor, params Term[] terms)
        {
            this.functor = functor;
            this.terms = new List<Term>(terms);
        }

        public AtomicFormula(string functor, IEnumerable<Term> terms)
        {
            this.functor = functor;
            this.terms = new List<Term>();
            this.terms.AddRange(terms);
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
            b.Append(functor);
            b.Append("(");
            for (byte i = 0; i < terms.Count; i++)
            {
                b.Append(terms[i].Name);
                if (i != terms.Count - 1)
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
            
            foreach (var t in terms)
            {
                if (!other.terms.Contains(t))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
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