//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  MusaModule.cs
//
//  Author:
//       davide <>
//
//  Copyright (c) 2016 davide
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
using System.Collections.Generic;
using System.Linq;

namespace MusaCommon
{
    public class MusaModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MusaCommon.MusaModule"/> class.
        /// </summary>
        /// <param name="Register">If set to <c>true</c> this module is
        /// automatically registered to container once is instanciated.</param>
        public MusaModule(bool Register = false)
        {
            if (Register == true)
                RegisterToModuleProvider(GetType());
        }

        #region Static methods

        private static ModuleProvider ModuleProvider
        {
            get { return ModuleProvider.Get(); }
        }

        /// <summary>
        /// Register the specified type to the module provider. The provided
        /// type must be related to a class that inherits from  MusaModule
        /// class and is decorated with [Register] attribute.
        ///
        /// TODO SISTEMARE LA DESCRIZIONE
        /// </summary>
        public static void RegisterToModuleProvider(Type type, Boolean singleton = false)
        {
            //Get the interfaces this module implements
            var intfc = type.GetInterfaces().ToList();

            //get the interface to be registered from the [Register([Type])] 
            //attribute
            var the_interface = getInterfaceFromAttribute(type);

            //check if interfaces this class implements contains the reference
            //interface specified as parameter of the [Register([Type])] 
            //attribute
            if (intfc.Contains(the_interface))
            {
                //Check if [the_inteface] is already mapped to a data type
                if (ModuleProvider.IsRegistered(the_interface))
                    return;

                //if not mapped, register the type to Unity container
                ModuleProvider.RegisterType(the_interface, type, singleton);
            }
        }

        public static void RegisterToModuleProvider(Type type, string name, Boolean singleton = false)
        {
            //Get the interfaces this module implements
            var intfc = type.GetInterfaces().ToList();

            //get the interface to be registered from the [Register([Type])] 
            //attribute
            var the_interface = getInterfaceFromAttribute(type);

            //check if interfaces this class implements contains the reference
            //interface specified as parameter of the [Register([Type])] 
            //attribute
            if (intfc.Contains(the_interface))
            {
                //Check if [the_inteface] is already mapped to a data type
                if (ModuleProvider.IsRegistered(the_interface))
                    return;

                //if not mapped, register the type to Unity container
                ModuleProvider.RegisterType(the_interface, type, name, singleton);
            }
        }

        //A MusaModule type must be decorated with RegisterAttribute
        //attribute which needs a Type specified.
        private static Type getInterfaceFromAttribute(Type type)
        {
            //get the reference interface from attribute
            var registered_attributes = from t in type.GetCustomAttributes(typeof(RegisterAttribute), true)
                let attributes = t as RegisterAttribute
                    where t != null
                select attributes.Type;

            if (registered_attributes.ToList().Count <= 0)
                throw new Exception("MusaModule class must be decorated with" +
                    "Register([Type]) attribute.");

            return registered_attributes.ElementAt(0);
        }

        #endregion Static methods
    }
}

