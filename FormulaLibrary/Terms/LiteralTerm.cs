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
    public class LiteralTerm : Term, IEquatable<LiteralTerm>
    {
        public LiteralTerm(string name)
            : base(name)
        {
        }

        public bool Equals(LiteralTerm other)
        {
            return Name.Equals(other.Name);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        
        public override bool Equals(object obj)
        {
            if(obj is LiteralTerm)
                return Equals((LiteralTerm)obj);
            return false;
        }
        
        public VariableTerm<type> toVariableTerm<type>(type value)
        {
            return new VariableTerm<type>(Name, value);
        }

        
    }
}