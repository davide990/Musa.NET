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

namespace FormulaLibrary
{
    public class VariableTerm<T> : Term, IUnifiable<T>, IEquatable<VariableTerm<T>>
    {
        /// <summary>
        /// The value of this term
        /// </summary>
        public T Value
        {
            get { return value; }
            private set { this.value = value; }
        }

        private T value;

        /// <summary>
        /// Create a new variable term
        /// </summary>
        /// <param name="name"></param>
        public VariableTerm(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Create a new variable term
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public VariableTerm(string name, T value)
            : base(name)
        {
            Value = value;
        }

        /// <summary>
        /// Return an equivalent literal term.
        /// </summary>
        public LiteralTerm toLiteralTerm()
        {
            return new LiteralTerm(Name);
        }

        /// <summary>
        /// Unify the value of this term with the given assignment. The unification
        /// succeeds only if the assignment's name is equal to the name of this term.
        /// </summary>
        public bool unify(Assignment<T> a)
        {
            if (!a.Name.Equals(Name))
                return false;
            
            Value = a.Value;
            return true;            
        }

        public override string ToString()
        {
            string shortTypeName = TypesMapping.getShortTypeName(Value.GetType().FullName);
            
            if (!shortTypeName.Equals("string") && !shortTypeName.Equals("bool"))
                return Name + "<-" + shortTypeName + "(" + Value.ToString() + ")";
            else
                return Name + "<-" + shortTypeName + "(\"" + Value.ToString().ToLower() + "\")";

        }

        public bool Equals(VariableTerm<T> other)
        {
            return  Name.Equals(other.Name) &&
            Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (!obj.GetType().Equals(typeof(VariableTerm<T>)))
                return false;

            return Equals(obj as VariableTerm<T>);
        }
    }
}