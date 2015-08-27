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
    public sealed class Assignment<T>
    {
        /// <summary>
        /// The name of this assignment. It coincide with the name of the term this assignment will assign its value to.
        /// </summary>
        public string Name
        {
            get { return name; }
        }
        private readonly string name;
        
        /// <summary>
        /// The value to assign
        /// </summary>
        public T Value
        {
            get { return value; }
            set { this.value = value; }
        }
        private T value;

        public Assignment(string name, T value )
        {
            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// Return a string representation for this assignment in the following form:
        /// 
        /// assign([name],[value])
        /// </summary>
        public override string ToString()
        {
            return "assign(" + Name + "," + Value + ")";
        }

        /// <summary>
        /// Check if this assignment is equals to <paramref name="obj"/>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Assignment<T>))
                return false;

            Assignment<T> other = (Assignment<T>)obj;
            return other.Name.Equals(Name) && other.Value.Equals(Value);
        }
    }
}