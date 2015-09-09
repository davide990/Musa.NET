/**
__  __                                     _   
|  \/  |                                   | |  
| \  / | _   _  ___   __ _     _ __    ___ | |_ 
| |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
| |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
|_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|

*/

using System;

namespace FormulaLibrary
{
    public class VariableTerm<T> : Term, IUnifiable<T>, IEquatable<VariableTerm<T>>
    {
        /// <summary>
        /// The value of this term
        /// </summary>
        public T Value
        {
            private set { this.value = value; }
            get { return value; }
        }
        private T value;

        /// <summary>
        /// Create a new variable term
        /// </summary>
        /// <param name="name"></param>
        public VariableTerm(string name)
            : base(name)
        {}

        /// <summary>
        /// Create a new variable term
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public VariableTerm(string name, T value)
            : base(name)
        {
            Value = value;
        }
        
        /// <summary>
        /// Return an equivalent literal term.
        /// </summary>
        public LiteralTerm toLiteralTerm()
        {
            return new LiteralTerm(Name);
        }
        
        /// <summary>
        /// Unify the value of this term with the given assignment. The unification
        /// succeeds only if the assignment's name is equal to the name of this term.
        /// </summary>
        public bool unify(Assignment<T> a)
        {
            if (!a.Name.Equals(Name))
                return false;
            
            Value = a.Value;
            return true;            
        }
        
        public override string ToString()
        {
            return "["+base.ToString()+","+Value.ToString()+"]";
        }

        public bool Equals(VariableTerm<T> other)
        {
            return  Name.Equals(other.Name) &&
                    Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (!obj.GetType().Equals(typeof(VariableTerm<T>)))
                return false;

            return Equals(obj as VariableTerm<T>);
        }

        
    }
}