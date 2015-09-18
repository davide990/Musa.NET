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
using System.Reflection;
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
    public sealed class AtomicFormula : Formula, ICloneable
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
        
        /// <summary>
        /// Return the number of terms this formula has
        /// </summary>
        public int TermsCount
        {
            get { return Terms.Count; }
        }

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

        


        /// <summary>
        /// Check if this formula is simple or parametric (that is, check if
        /// this formula contains or not variable terms)
        /// </summary>
        public override bool IsParametric()
        {
            foreach (Term t in terms)
            {
                if (!(t is LiteralTerm))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Convert this formula to a simple type formula. That is, if this formula
        /// contains any variable term, convert them to literal terms. Found variable
        /// terms are returned to the output list
        /// </summary>
        /// <returns>a list containing the variable terms this formula previously contained</returns>
        public List<object> ConvertToSimpleFormula()
        {
            List<object> variableTerms = new List<object>();
            
            //Iterate each term
            for (int i=0;i<terms.Count;i++)
            {
                //if a variable term occurs
                if(terms[i].GetType().IsGenericType)
                {
                    //add the variable term to the output list
                    variableTerms.Add(terms[i]);

                    //get the type info for the current term
                    Type variableTermType = typeof(VariableTerm<>).MakeGenericType(terms[i].GetType().GetGenericArguments()[0]);
                    MethodInfo parse_method = variableTermType.GetMethod("toLiteralTerm");
                    
                    //convert to literal term
                    terms[i] = (LiteralTerm)parse_method.Invoke(terms[i], new object[] { });
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
            for (int i = 0; i < terms.Count; i++)
            {
                //if a variable term occurs
                if (terms[i].GetType().IsGenericType)
                { 
                    //get the type info for the current term
                    Type variableTermType = typeof(VariableTerm<>).MakeGenericType(terms[i].GetType().GetGenericArguments()[0]);
                    ConstructorInfo cinfo = variableTermType.GetConstructor(new[] { typeof(string), terms[i].GetType().GetGenericArguments()[0] });

                    //get the value of the current term
                    object value = variableTermType.GetProperty("Value").GetValue(terms[i]);
                    value = Convert.ChangeType(value, terms[i].GetType().GetGenericArguments()[0]);

                    //create a new variable term instance
                    object varTerm = cinfo.Invoke(new[] { terms[i].Name, value });

                    //add the new instance to the cloned formula
                    clone.terms.Add((Term)varTerm);
                }
                else
                {
                    clone.terms.Add(new LiteralTerm(terms[i].Name));
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