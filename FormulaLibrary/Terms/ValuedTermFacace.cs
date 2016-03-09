//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  ValuedTermFacace.cs
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
using System.Reflection;
using MusaCommon;

namespace FormulaLibrary
{
    [Register(typeof(IValuedTermFacade))]
    public class ValuedTermFacace : MusaModule, IValuedTermFacade
    {
        private ILogger Logger
        {
            get;
            set;
        }

        public ValuedTermFacace()
        {
            Logger = ModuleProvider.Get().Resolve<ILogger>();
        }

        /// <summary>
        /// Gets the variable generic term for the specified type.
        /// </summary>
        /// <returns>The variable term for type t.</returns>
        /// <param name="t">The type the variable term supports.</param>
        public Type GetVariableTermFor(Type t)
        {
            return typeof(ValuedTerm<>).MakeGenericType(t);
        }


        /// <summary>
        /// Gets the value of variable term.
        /// </summary>
        /// <returns>The value of variable term.</returns>
        /// <param name="term">The term.</param>
        public object GetValueOfVariableTerm(ITerm term)
        {
            return GetValueFromGeneric(term, "Value");
        }

        private object GetValueFromGeneric(ITerm term, string field_name)
        {
            if (term.IsLiteral())
            {
                Logger.SetColorForNextConsoleLog(ConsoleColor.Black, ConsoleColor.DarkRed);
                Logger.Log(LogLevel.Error, "Provided term is not a valued term.");
                throw new Exception("Provided term is not a valued term.");
            }

            try
            {
                return term.GetType().GetProperty(field_name).GetValue(term);
            }
            catch (Exception e)
            {
                Logger.SetColorForNextConsoleLog(ConsoleColor.Black, ConsoleColor.DarkRed);
                Logger.Log(LogLevel.Error, "Failed to get name of valued term '" + term + "'.Error: " + e.Message);
                return null;
            }
        }

        /// <summary>
        /// Create a variable term.
        /// </summary>
        /// <returns>The variable term for.</returns>
        /// <param name="Name">The name of the variable term.</param>
        /// <param name="value">The value of the variable term.</param>
        public ITerm CreateValuedTerm(object value)
        {
            //Get the type of the specified variable term value
            Type varTermValueType = value.GetType();

            //Get a variable term type for the specified input type
            Type varTermType = GetVariableTermFor(varTermValueType);

            //Get the constructor for the variable term
            ConstructorInfo cinfo = varTermType.GetConstructor(new[] { varTermValueType });

            return cinfo.Invoke(new[] { value }) as ITerm;
        }

    }
}

