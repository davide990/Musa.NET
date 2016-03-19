//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  NotFormula.cs
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
using MusaCommon;

namespace FormulaLibrary
{
    public sealed class NotFormula : Formula, INotFormula, IEquatable<NotFormula>
    {
        public Formula Formula
        {
            get;
            private set;
        }

        public NotFormula(Formula f)
        {
            Formula = f;
        }

        public override void Unify(List<IAssignment> assignment)
        {
            Formula.Unify(assignment);
        }

        public override FormulaType GetFormulaType()
        {
            return FormulaType.NOT_FORMULA;
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.Append("!(");
            b.Append(Formula.ToString());
            b.Append(")");
            return b.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is NotFormula)
                return Equals(obj as NotFormula);
            return false;
        }

        public bool Equals(NotFormula other)
        {
            return other.Formula.Equals(Formula);
        }

        public override int GetHashCode()
        {
            return -Formula.GetHashCode();
        }

        public override bool IsParametric()
        {
            return Formula.IsParametric();
        }

        public override bool IsAtomic()
        {
            return false;
        }

        public IFormula GetFormula()
        {
            return Formula;
        }
    }
}