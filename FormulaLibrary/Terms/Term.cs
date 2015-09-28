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
        private readonly string name;
        public string Name
        {
            get { return name; }
            private set { }
        }

        public Term(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return name;
        }
    }
}