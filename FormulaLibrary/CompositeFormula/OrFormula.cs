//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  OrFormula.cs
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
using System.Linq;
using System.Collections.Generic;
using MusaCommon;

namespace FormulaLibrary
{
    public sealed class OrFormula : Formula, IOrFormula, IEquatable<OrFormula>
    {
        public IFormula Left
        {
            get;
            private set;
        }

        public IFormula Right
        {
            get;
            private set;
        }

        public OrFormula(IFormula Left, IFormula Right)
        {
            this.Left = Left;
            this.Right = Right;
        }

        public IFormula this[string s]
        {
            get
            {
                if (s.ToLower().Equals("Left"))
                    return Left;
                else
                    return Right;
            }
        }

        public IFormula this[byte s]
        {
            get
            {
                if (s == 0)
                    return Left;
                else
                    return Right;
            }
        }

        public override void Unify(List<IAssignment> assignment)
        {
            Right.Unify(assignment);
            Left.Unify(assignment);
        }

        public bool Equals(OrFormula other)
        {
            return Left.Equals(other.Left) && Right.Equals(other.Right);
        }

        public override bool Equals(object obj)
        {
            if (obj is OrFormula)
                return Equals(obj as OrFormula);
            return false;
        }

        public override int GetHashCode()
        {
            return Left.GetHashCode() + Right.GetHashCode();
        }

        public override FormulaType GetFormulaType()
        {
            return FormulaType.OR_FORMULA;
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
            b.Append("|");
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

        public override bool MatchWith(IFormula f, out List<IAssignment> generatedAssignment)
        {
            var leftAssignments = new List<IAssignment>();
            var rightAssignments = new List<IAssignment>();

            var leftMatch = f.MatchWith(Left, out leftAssignments);
            var rightMatch = f.MatchWith(Right, out rightAssignments);
            generatedAssignment = leftAssignments.Union(rightAssignments).ToList();

            return leftMatch | rightMatch;
        }

        public override object Clone()
        {
            return new OrFormula(Left.Clone() as IFormula, Right.Clone() as IFormula);
        }


        public override string GetSource()
        {
            return Left.GetSource();
        }

        public override void SetSource(string source)
        {
            Left.SetSource(source);
            Right.SetSource(source);
        }
    }
}