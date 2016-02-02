//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  EventArgs.cs
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
using System;
using System.Collections.Generic;
using MusaCommon;

namespace MusaCommon
{
    //TODO RINOMINA QUESTA CLASSE

    /// <summary>
    /// Event arguments.
    /// </summary>
    public class AgentEventArgs : IAgentEventArgs
    {
        readonly Dictionary<string,string> internalDict = new Dictionary<string,string>();

        public String Name { get; set; }

        public void Add(string key, string value)
        {
            internalDict.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return internalDict.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return internalDict.Keys; }
        }

        public bool Remove(string key)
        {
            return internalDict.Remove(key);
        }

        public bool TryGetValue(string key, out string value)
        {
            return internalDict.TryGetValue(key, out value);
        }

        public ICollection<string> Values
        {
            get { return internalDict.Values; }
        }

        public string this [string key]
        {
            get
            {
                return internalDict[key];
            }
            set
            {
                internalDict[key] = value;
            }
        }

        #region ICollection<KeyValuePair<string,string>> Members

        /// <Docs>The item to add to the current collection.</Docs>
        /// <para>Adds an item to the current collection.</para>
        /// <remarks>To be added.</remarks>
        /// <exception cref="System.NotSupportedException">The current collection is read-only.</exception>
        /// <summary>
        /// Add the specified item.
        /// </summary>
        /// <param name="item">The item to be added to this collection. The key
        /// represents the unique name of the argument, while the string is the
        /// value of the argument.</param>
        public void Add(KeyValuePair<string, string> item)
        {
            internalDict.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            internalDict.Clear();
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            return (internalDict.ContainsKey(item.Key) && internalDict.ContainsValue(item.Value));
        }

        public int Count
        {
            get { return internalDict.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,string>> Members

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {    
            return internalDict.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return internalDict.GetEnumerator();
        }

        #endregion

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
    }
}

