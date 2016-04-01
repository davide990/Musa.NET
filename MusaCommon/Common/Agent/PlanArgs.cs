//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  PlanArgs.cs
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
using System.Collections.Generic;
using MusaCommon;

namespace MusaCommon
{
    /// <summary>
    /// Event arguments.
    /// </summary>
    public class PlanArgs : IPlanArgs
    {
        readonly Dictionary<string, object> internalDict = new Dictionary<string, object>();

        public String Name { get; set; }


        #region Custom methods

        public T GetArg<T>(string key)
        {
            object the_value;
            TryGetValue(key, out the_value);
            if (the_value != null)
                return (T)the_value;
            return default(T);
        }

        public object GetArg(string key)
        {
            object the_value;
            TryGetValue(key, out the_value);
            return the_value;
        }

        public bool ExistsArg(string key)
        {
            return ContainsKey(key);
        }

        public Type TypeOfArg(string key)
        {
            if (!ExistsArg(key))
                return null;

            return GetArg(key).GetType();

        }

        public ICollection<object> GetAllArgs()
        {
            return Values;
        }

        #endregion Custom methods

        public void Add(string key, object value)
        {
            internalDict.Add(key, value);
        }


        public void Add(params IAssignment[] assignments)
        {
            foreach (IAssignment a in assignments)
                internalDict.Add(a.GetName(), a.GetValue());
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

        public bool TryGetValue(string key, out object value)
        {
            return internalDict.TryGetValue(key, out value);
        }

        public ICollection<object> Values
        {
            get { return internalDict.Values; }
        }

        public object this[string key]
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
        public void Add(KeyValuePair<string, object> item)
        {
            internalDict.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            internalDict.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
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

        public bool Remove(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,object>> Members

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
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

        /// <summary>
        /// Merge all items in the input AgentEventArgs object to this collection.
        /// </summary>
        /// <param name="args">Arguments.</param>
        public void Merge(PlanArgs args)
        {
            foreach (KeyValuePair<string, object> kvp in args)
                Add(kvp);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
    }
}

