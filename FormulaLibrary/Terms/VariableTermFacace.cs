//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  VariableTermFacace.cs
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
    public static class VariableTermFacace
    {
        private static ILogger Logger
        {
            get { return ModuleProvider.Get().Resolve<ILogger>(); }
        }

        /// <summary>
        /// Gets the variable generic term for the specified type.
        /// </summary>
        /// <returns>The variable term for type t.</returns>
        /// <param name="t">The type the variable term supports.</param>
        public static Type GetVariableTermFor(Type t)
        {
            return typeof(VariableTerm<>).MakeGenericType(t);
        }

        /// <summary>
        /// Gets the name of variable term.
        /// </summary>
        /// <returns>The name of variable term.</returns>
        /// <param name="var_term">Variable term.</param>
        public static object GetNameOfVariableTerm(object var_term)
        {
            return GetValueFromGeneric(var_term, "Name");
        }

        /// <summary>
        /// Gets the value of variable term.
        /// </summary>
        /// <returns>The value of variable term.</returns>
        /// <param name="var_term">Variable term.</param>
        public static object GetValueOfVariableTerm(object var_term)
        {
            return GetValueFromGeneric(var_term, "Value");
        }

        private static object GetValueFromGeneric(object var_term, string field_name)
        {
            Type var_term_type = var_term.GetType();

            if (!var_term_type.IsSubclassOf(typeof(Term)) || !var_term_type.IsGenericType)
            {
                Logger.Log(LogLevel.Error, "Provided object is not a term, or it is not a variable term.");
                throw new Exception("Provided object is not a term.");
            }

            try
            {
                return var_term_type.GetProperty(field_name).GetValue(var_term);
                //return Convert.ChangeType(var_term_type.GetProperty(field_name).GetValue(var_term), typeof(string));    
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, "Failed to get name of variable term '" + var_term + "'.Error: " + e.Message);
                return null;
            }
        }

        /// <summary>
        /// Converts a variable term to a literal term.
        /// </summary>
        /// <returns>The literal term corresponding to [var_term].</returns>
        /// <param name="var_term">The variable term to be converted.</param>
        public static LiteralTerm ConvertToLiteralTerm(Term var_term)
        {
            if (!var_term.GetType().IsGenericType)
            {
                Logger.Log(LogLevel.Warn, "Provided term [" + var_term + "] is not a variable term.");
                return (LiteralTerm)var_term;
            }

            MethodInfo parse_method = var_term.GetType().GetMethod("toLiteralTerm");
            return (LiteralTerm)parse_method.Invoke(var_term, new object[] { });
        }

        /// <summary>
        /// Create a variable term.
        /// </summary>
        /// <returns>The variable term for.</returns>
        /// <param name="Name">The name of the variable term.</param>
        /// <param name="value">The value of the variable term.</param>
        public static object CreateVariableTerm(string Name, object value)
        {
            //Get the type of the specified variable term value
            Type varTermValueType = value.GetType();

            //Get a variable term type for the specified input type
            Type varTermType = GetVariableTermFor(varTermValueType);

            //Get the constructor for the variable term
            ConstructorInfo cinfo = varTermType.GetConstructor(new[] { typeof(string), varTermValueType });

            return cinfo.Invoke(new[] { Name, value });
        }

        public static object SetValueForVariableTerm(object varTerm, object value)
        {
            if (!varTerm.GetType().IsSubclassOf(typeof(Term)) || !varTerm.GetType().IsGenericType)
            {
                Logger.Log(LogLevel.Error, "Provided object is not a term, or it is not a variable term.");
                throw new Exception("Provided object is not a term.");
            }

            //Get the actual value type of the provided term
            Type varTermValueType = varTerm.GetType().GetGenericArguments()[0];

            //Check if variable term value type is compatible with the specified value
            if (!value.GetType().IsEquivalentTo(varTermValueType))
            {
                Logger.Log(LogLevel.Error, "Provided value is incompatible with variable term type. [" + value.GetType().FullName + " != " + varTermValueType.FullName + "]");
                throw new Exception("Provided value is incompatible with variable term type. [" + value.GetType().FullName + " != " + varTermValueType.FullName + "]");
            }

            varTerm.GetType().GetProperty("Value").SetValue(varTerm, value);
            return varTerm;
        }
    }
}

