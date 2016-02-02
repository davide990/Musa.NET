//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  IPlanCollection.cs
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
using System.Collections;

namespace MusaCommon
{
    public interface IPlanCollection : IDictionary<Type, IPlanInstance>
    {
        /*void Add(Type key, IPlanInstance value) ;
        void Add(KeyValuePair<Type, IPlanInstance> item);
        void Clear();
        bool Contains(KeyValuePair<Type, IPlanInstance> item);
        bool ContainsKey(Type key);
        bool Remove(Type key);
        bool Remove(KeyValuePair<Type, IPlanInstance> item);
        IEnumerator<KeyValuePair<Type, IPlanInstance>> GetEnumerator();
        bool TryGetValue(Type key, out IPlanInstance value);
        void CopyTo(KeyValuePair<Type, IPlanInstance>[] array, int arrayIndex);*/
    }
}

