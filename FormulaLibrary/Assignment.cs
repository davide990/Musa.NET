/**
__  __                                     _   
|  \/  |                                   | |  
| \  / | _   _  ___   __ _     _ __    ___ | |_ 
| |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
| |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
|_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|

*/
using System;
namespace FormulaLibrary
{
    /// <summary>
    /// The only purpose of this class is to made possible to create containers of assignment of different types. In fact, since
    /// Assignment is a template class, is not possible to create directly lists or other containes objects containing them. So,
    /// by doing this separation between Assignment and AssignmentType it is possible to create a list of AssignmentType, and adding
    /// to it any assignment.
    /// </summary>
    public abstract class AssignmentType
    {
        /// <summary>
        /// The name of this assignment. It coincide with the name of the term this assignment will assign its value to.
        /// </summary>
        public string Name
        {
            get { return name; }
        }
        private readonly string name;
        
        public AssignmentType(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Create a new assignment
        /// </summary>
        /// <returns></returns>
        public static AssignmentType CreateAssignmentForTerm(string assignmentName, object value, Type type)
        {
            Type varTermType = typeof(Assignment<>).MakeGenericType(type);
            return (AssignmentType)Activator.CreateInstance(varTermType, assignmentName, value);
        }
    }

    public sealed class Assignment<T> : AssignmentType
    {
        /// <summary>
        /// The value to assign
        /// </summary>
        public T Value
        {
            get { return value; }
        }
        private T value;

        public Assignment(string name) : base(name)
        {
        }

        public Assignment(string name, T value) : base(name)
        {
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

        public void setValue(T value)
        {
            this.value = value;
        }
    }
}