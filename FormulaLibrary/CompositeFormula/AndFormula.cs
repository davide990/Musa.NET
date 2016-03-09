//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  AndFormula.cs
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
using System.Collections.Generic;
using System.Linq;
using MusaCommon;

namespace FormulaLibrary
{
    public sealed class AndFormula : Formula, IAndFormula, IEquatable<AndFormula>
    {
        public Formula Left
        {
            get;
            private set;
        }

        public Formula Right
        {
            get;
            private set;
        }

        public AndFormula(Formula Left, Formula Right)
        {
            this.Left = Left;
            this.Right = Right;
        }

        public Formula this [string s]
        {
            get
            {
                if (s.ToLower().Equals("Left"))
                    return Left;
                else
                    return Right;
            }
        }

        public Formula this [byte s]
        {
            get
            {
                if (s == 0)
                    return Left;
                else
                    return Right;
            }
        }

        public bool Equals(AndFormula other)
        {
            return Left.Equals(other.Left) && Right.Equals(other.Right);
        }

        public override bool Equals(object obj)
        {
            if (obj is AndFormula)
                return Equals(obj as AndFormula);
            return false;
        }

        public override FormulaType GetFormulaType()
        {
            return FormulaType.AND_FORMULA;
        }

        public override bool IsParametric()
        {
            return Left.IsParametric() & Right.IsParametric();
        }

        public override bool IsAtomic()
        {
            return false;
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.Append("(");
            b.Append(Left.ToString());
            b.Append("&");
            b.Append(Right.ToString());
            b.Append(")");
            return b.ToString();
        }
            
        public IFormula GetLeft()
        {
            return Left;
        }

        public IFormula GetRight()
        {
            return Right;
        }
    }
}