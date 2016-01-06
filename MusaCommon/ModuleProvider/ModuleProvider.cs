//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  ModuleProvider.cs
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
using Microsoft.Practices.Unity;
using System.Linq;

namespace MusaCommon
{
    /// <summary>
    /// A Module provider used to inject dependencies. It helps reducing the
    /// dependencies between projects.
    /// </summary>
    public class ModuleProvider
    {
        /// <summary>
        /// The ModuleProvider instance.
        /// </summary>
        private static ModuleProvider instance;

        /// <summary>
        /// Gets or sets the Unity container.
        /// </summary>
        /// <value>The container.</value>
        private UnityContainer container { get; set; }

        /// <summary>
        /// Get the module provider.
        /// </summary>
        public static ModuleProvider Get()
        {
            if (instance == null)
                instance = new ModuleProvider();

            return instance;
        }

        private ModuleProvider()
        {
            container = new UnityContainer();
        }

        /// <summary>
        /// Registers a type mapping.
        /// </summary>
        /// <param name="Interface">Interface.</param>
        /// <param name="Implementation">Implementation.</param>
        /// <param name="singleton">If set to <c>true</c> singleton.</param>
        public void RegisterType(Type Interface, Type Implementation, bool singleton = false)
        {
            if (singleton)
                container.RegisterType(Interface, Implementation, new ContainerControlledLifetimeManager());
            else
                container.RegisterType(Interface, Implementation);
        }

        /// <summary>
        /// Resolve an instance of the requested type.
        /// </summary>
        /// <typeparam name="T">The type to be resolved.</typeparam>
        public T Resolve<T>()
        {
            //Resolve the type T
            var the_instance = container.Resolve<T>();

            //Check for properties, within T, marked with [Dependency] attribute
            var props = the_instance.GetType().GetProperties().Where(
                            prop => Attribute.IsDefined(prop, typeof(DependencyAttribute)));

            //Resolve each property decorated with [Dependency] attribute
            foreach (var Property in props)
                Property.SetValue(the_instance, container.Resolve(Property.PropertyType));

            //return the resolved instance
            return the_instance;
        }

    }
}

