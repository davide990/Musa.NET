//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  FormulaUtils.cs
//
//  Author:
//       Davide Guastella <davide.guastella90@gmail.com>
//
//  Copyright (c) 2015 Davide Guastella
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
using System.Collections.Generic;

namespace FormulaLibrary
{
	public static class FormulaUtils
	{
		/// <summary>
		/// Given a generic formula, return its inner atomic formulas.
		/// </summary>
		public static List<AtomicFormula> UnrollFormula(Formula f)
		{
			List<AtomicFormula> unrolled_formula_list = new List<AtomicFormula>();

			if (f is AndFormula)
			{
				unrolled_formula_list.AddRange(UnrollFormula((f as AndFormula).Left));
				unrolled_formula_list.AddRange(UnrollFormula((f as AndFormula).Right));
			}
			else if (f is OrFormula)
			{
				unrolled_formula_list.AddRange(UnrollFormula((f as OrFormula).Left));
				unrolled_formula_list.AddRange(UnrollFormula((f as OrFormula).Right));
			}
			else if (f is NotFormula)
			{
				unrolled_formula_list.AddRange(UnrollFormula((f as NotFormula).Formula));
			}
			else
				unrolled_formula_list.Add(f as AtomicFormula);

			return unrolled_formula_list;
		}
	}
}

