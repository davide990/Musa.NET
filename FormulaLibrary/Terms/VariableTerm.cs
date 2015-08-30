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

        public bool Equals<type>(VariableTerm<type> other)
        {
            return Value is type && other.Name.Equals(Name) && other.Value.Equals(Value);
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
            return "("+base.ToString()+","+Value.ToString()+")";
        }
    }
}