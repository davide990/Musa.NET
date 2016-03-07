//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  FormulaLibraryInitializer.cs
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
    public static class FormulaLibraryInitializer
    {
        public static void Initialize()
        {
            //var formula_parser_specification = Tuple.Create(typeof(FormulaParser), true, string.Empty);

            var formula_utils_specification = Tuple.Create(typeof(FormulaUtils), true, string.Empty);
            var assignment_factory_specification = Tuple.Create(typeof(AssignmentFactory), true, string.Empty);

            new ModuleInitializer().InitializeThisModule(formula_utils_specification, assignment_factory_specification);
        }
    }
}

