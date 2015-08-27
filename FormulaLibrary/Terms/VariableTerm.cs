
using System;
/**
__  __                                     _   
|  \/  |                                   | |  
| \  / | _   _  ___   __ _     _ __    ___ | |_ 
| |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
| |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
|_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|

*/
namespace FormulaLibrary
{
    public class VariableTerm<T> : Term, IUnifiable<T>
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

        public VariableTerm(string name)
            : base(name)
        {}
        public VariableTerm(string name, T value)
            : base(name)
        {
            Value = value;
        }

        public LiteralTerm toLiteralTerm()
        {
            return (LiteralTerm)Convert.ChangeType(this, typeof(LiteralTerm));
        }
        
        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(Term))
            {
                VariableTerm<T> other = (VariableTerm<T>)obj;
                return Name.Equals(other.Name) && other.Value.Equals(Value);
            }
            return false;
        }

        public void unify(Assignment<T> a)
        {
            Value = a.Value;
        }
    }
}