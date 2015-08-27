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
    public sealed class NotFormula : Formula
    {
        public Formula Formula
        {
            get { return formula; }
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
            b.Append("not(");
            b.Append(formula.ToString());
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