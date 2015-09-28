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
    public sealed class NotFormula : Formula, IEquatable<NotFormula>
    {
        public Formula Formula
        {
            get { return formula; }
            private set { }
        }
        private readonly Formula formula;

        public NotFormula(Formula f)
        {
            formula = f;
        }

        public override FormulaType getType()
        {
            return FormulaType.NOT_FORMULA;
        }
        
        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.Append("!(");
            b.Append(formula.ToString());
            b.Append(")");
            return b.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is NotFormula)
                return Equals(obj as NotFormula);
            return false;
        }

        public bool Equals(NotFormula other)
        {
            return other.Formula.Equals(Formula);
        }

        public override bool IsParametric()
        {
            return Formula.IsParametric();
        }
    }
}