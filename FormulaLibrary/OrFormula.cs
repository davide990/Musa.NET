﻿/**
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
    public sealed class OrFormula : Formula
    {
        private readonly Formula left;
        private readonly Formula right;

        public OrFormula(Formula left, Formula right)
        {
            this.left = left;
            this.right = right;
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
                else return right;
            }
        }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override FormulaType getType()
        {
            return FormulaType.OR_FORMULA;
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.Append("or(");
            b.Append(left.ToString());
            b.Append(",");
            b.Append(right.ToString());
            b.Append(")");
            return b.ToString();
        }
        
    }
}