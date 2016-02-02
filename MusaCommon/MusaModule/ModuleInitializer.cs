//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  ModuleInitializer.cs
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
using System.Linq;
using System.Collections.Generic;

namespace MusaCommon
{
    public sealed class ModuleInitializer
    {
        /*public event EventHandler BeforeInitialize;
        public event EventHandler AfterInitialize;*/

        public void InitializeThisModule()
        {
            /*if (BeforeInitialize != null)
                BeforeInitialize.Invoke();*/

            //Get the calling assembly
            var calling_assembly = System.Reflection.Assembly.GetCallingAssembly();

            //Get the list of classes from the assembly
            var class_list = calling_assembly.GetExportedTypes().Where(t => t.IsClass);

            //Filter the class list by getting only those subclassing MusaModule
            var modules_list = class_list.Where(t => t.IsSubclassOf(typeof(MusaModule)));

            //Register the module to Unity container
            foreach(var T in modules_list)
                MusaModule.RegisterToModuleProvider(T);
            
            /*if (AfterInitialize != null)
                AfterInitialize.Invoke();*/
        }

        /// <summary>
        /// Initializes the this module.
        /// </summary>
        /// <param name="Specifications">A set of specifications for each
        /// module to be registered. Each specification tuple is composed of a
        /// reference type, a boolean value that indicates if the type has to be
        /// registered as singleton, and a string value to be associated to the
        /// module.</param>
        public void InitializeThisModule(params Tuple<Type, Boolean, string>[] Specifications)
        {
            
            /*if (BeforeInitialize != null)
                BeforeInitialize.Invoke();*/

            //Get the calling assembly
            var calling_assembly = System.Reflection.Assembly.GetCallingAssembly();

            //Get the list of classes from the assembly
            var class_list = calling_assembly.GetExportedTypes().Where(t => t.IsClass);

            //Filter the class list by getting only those subclassing MusaModule
            var modules_list = class_list.Where(t => t.IsSubclassOf(typeof(MusaModule)));

            var modules_spec = Specifications.ToList();

            //Register the module to Unity container
            foreach(var T in modules_list)
            {
                var this_module_spec = modules_spec.Find(x => T.IsEquivalentTo(x.Item1));

                //If a specification has been provided for module T, use it
                if(this_module_spec != null)
                {
                    if(!string.IsNullOrEmpty(this_module_spec.Item3))
                        MusaModule.RegisterToModuleProvider(T, this_module_spec.Item3, this_module_spec.Item2);
                    else
                        MusaModule.RegisterToModuleProvider(T, this_module_spec.Item2);
                }
                else
                    MusaModule.RegisterToModuleProvider(T);
            }
                
            /*if (AfterInitialize != null)
                AfterInitialize.Invoke();*/
        }
    }
}

