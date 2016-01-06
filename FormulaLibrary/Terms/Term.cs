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
    public abstract class Term
    {
        /// <summary>
        /// This term's name
        /// </summary>
        public string Name{ get; private set; }

        public Term(string Name)
        {
            this.Name = Name;
        }

        public override string ToString()
        {
            return Name;
        }


    }
}