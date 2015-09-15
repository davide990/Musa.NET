/**
         __  __                                     _   
        |  \/  |                                   | |  
        | \  / | _   _  ___   __ _     _ __    ___ | |_ 
        | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
        | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
        |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|

*/
using System;
using System.Text;

namespace FormulaLibrary
{
    public sealed class AndFormula : Formula,IEquatable<AndFormula>
    {
        private readonly Formula left;
        private readonly Formula right;

        public AndFormula(Formula left, Formula right)
        {
            this.left   = left;
            this.right  = right;
        }

        public Formula this[string s]
        {
            get
            {
                if (s.ToLower().Equals("left")) return left;
                else return right;
            }
        }

        public Formula this[byte s]
        {
            get
            {
                if (s == 0) return left;
                else        return right;
            }
        }

        public bool Equals(AndFormula other)
        {
            return left.Equals(other.left) && right.Equals(other.right);
        }

        public override bool Equals(object obj)
        {
            if (obj is AndFormula)
                return Equals(obj as AndFormula);
            return false;
        }

        public override FormulaType getType()
        {
            return FormulaType.AND_FORMULA;
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.Append("and(");
            b.Append(left.ToString());
            b.Append(",");
            b.Append(right.ToString());
            b.Append(")");
            return b.ToString();
        }
    }
}