//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  VariableTerm.cs
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
using System.Text;

namespace FormulaLibrary
{
    public class ValuedTerm<T> : Term, IEquatable<ValuedTerm<T>>
    {
        /// <summary>
        /// The value of this term
        /// </summary>
        public T Value
        {
            get;
            private set;
        }

        /// <summary>
        /// Create a new variable term
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public ValuedTerm(T value)
            : base("")
        {
            Value = value;
        }

        public override string ToString()
        {
            if (typeof(T) == typeof(string))
                return "\"" + Value + "\"";
            
            return Value.ToString();
        }

        public bool Equals(ValuedTerm<T> other)
        {
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (!obj.GetType().Equals(typeof(ValuedTerm<T>)))
                return false;

            return Equals(obj as ValuedTerm<T>);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool IsLiteral()
        {
            return false;
        }

        public override object GetValue()
        {
            return Value;
        }

        public override string GetName()
        {
            return Name;
        }
    }
}