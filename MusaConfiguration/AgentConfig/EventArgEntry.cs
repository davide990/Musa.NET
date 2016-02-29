//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  EventArgEntry.cs
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
using System.Xml.Serialization;
using System;
using MusaCommon;

namespace MusaConfiguration
{
    public class EventArgEntry
    {
        /// <summary>
        /// Gets or sets the name of the argument.
        /// </summary>
        /// <value>The name.</value>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the argument.
        /// </summary>
        /// <value>The value.</value>
        [XmlAttribute("Value")]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the type of the value for this event arg.
        /// </summary>
        /// <value>The type.</value>
        [XmlAttribute("Type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets the data type of the value.
        /// </summary>
        /// <value>The type of the value.</value>
        [XmlIgnore]
        public Type ValueType
        {
            get
            {
                if (string.IsNullOrEmpty(Type))
                    return typeof(string);
                else
                    return System.Type.GetType(TypesMapping.getCompleteTypeName(Type.ToLower()));
            }
        }
    }
}

