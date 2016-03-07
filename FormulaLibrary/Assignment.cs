//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  Assignment.cs
//
//  Author:
//       Davide Guastella <davide.guastella90@gmail.com>
//
//  Copyright (c) 2016 Davide Guastella
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using MusaCommon;

namespace FormulaLibrary
{
    /* /// <summary>
    /// The only purpose of this class is to make possible to create containers of assignment of different types. In fact, since
    /// Assignment is a template class, is not possible to create directly lists or other containes objects containing them. So,
    /// by doing this separation between Assignment and AssignmentType it is possible to create a list of AssignmentType, and adding
    /// to it any assignment.
    /// </summary>
    public abstract class AssignmentType
    {

        /// <summary>
        /// Create a new assignment
        /// </summary>
        /// <returns></returns>
        public static AssignmentType CreateAssignmentForTerm(string termName, object value, Type type)
        {
            Type varTermType = typeof(Assignment<>).MakeGenericType(type);
            return (AssignmentType)Activator.CreateInstance(varTermType, termName, value);
        }
    }*/

    //public sealed class Assignment<T> : AssignmentType
    public sealed class Assignment<T> : IAssignment
    {
        /// <summary>
        /// The name of this assignment. It coincide with the name of the term this assignment will assign its value to.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// The value to assign
        /// </summary>
        public T Value
        {
            get;
            private set;
        }

        public Assignment(string termName)
        {
            Name = termName;
        }

        public Assignment(string termName, T value)
        {
            Name = termName;
            Value = value;
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

        #region IAssignment methods

        public string GetName()
        {
            return Name;
        }

        public object GetValue()
        {
            return Value;
        }

        #endregion IAssignment methods
    }
}