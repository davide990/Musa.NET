/**
__  __                                     _   
|  \/  |                                   | |  
| \  / | _   _  ___   __ _     _ __    ___ | |_ 
| |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
| |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
|_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|

*/
using MusaCommon;
using System.Collections.Generic;


namespace FormulaLibrary
{
    public abstract class Formula : IFormula
    {
        public abstract FormulaType GetFormulaType();
        public abstract bool IsParametric();
        public abstract bool IsAtomic();
        public abstract override string ToString();
        public abstract override bool Equals(object obj);
    }
}