//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  LiteralTerm.cs
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
    public class LiteralTerm : Term, IUnifiable, IEquatable<LiteralTerm>
    {
        public LiteralTerm(string name)
            : base(name)
        {
        }

        public bool Equals(LiteralTerm other)
        {
            return Name.Equals(other.Name);
        }

        public override bool Equals(object obj)
        {
            if (obj is LiteralTerm)
                return Equals((LiteralTerm)obj);
            return false;
        }

        public ValuedTerm<T> Unify<T>(Assignment<T> a)
        {
            return new ValuedTerm<T>(a.Value);
        }

        public override bool IsLiteral()
        {
            return true;
        }

        public override object GetValue()
        {
            return null;
        }

        public override string GetName()
        {
            return Name;
        }
    }
}