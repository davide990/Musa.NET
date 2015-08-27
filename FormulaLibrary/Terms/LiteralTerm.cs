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
    public class LiteralTerm : Term
    {
        public LiteralTerm(string name)
            : base(name)
        {
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(Term))
                return ((Term)obj).ToString().Equals(base.ToString());
            return false;
        }
        
    }
}