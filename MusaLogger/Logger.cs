//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  Logger.cs
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

using System.Collections.Generic;
using MusaCommon;
using System;
using System.Linq;

namespace MusaLogger
{
    /// <summary>
    /// Logger.
    /// </summary>
    [Register(typeof(ILogger))]
    public class Logger : MusaModule, ILogger
    {
        /// <summary>
        /// Gets the fragments of this logger. Each fragment is responsible for
        /// logging to a specific output.
        /// </summary>
        /// <value>The fragments.</value>
        public List<ILoggerFragment> Fragments
        {
            get;
            private set;
        }

        public MongoDBLoggerFragment MongoDBLogger
        { 
            get
            { 
                if (Fragments == null)
                    return null;
                if (Fragments.Count == 0)
                    return null;
                return Fragments.Find(x => x is MongoDBLoggerFragment) as MongoDBLoggerFragment; 
            } 
        }

        public ConsoleLoggerFragment ConsoleLogger
        { 
            get
            { 
                if (Fragments == null)
                    return null;
                if (Fragments.Count == 0)
                    return null;
                return Fragments.Find(x => x is ConsoleLoggerFragment) as ConsoleLoggerFragment; 
            } 
        }

        public FileLoggerFragment FileLogger
        { 
            get
            { 
                if (Fragments == null)
                    return null;
                if (Fragments.Count == 0)
                    return null;
                return Fragments.Find(x => x is FileLoggerFragment) as FileLoggerFragment; 
            }
        }

        public WCFLoggerFragment WCFLogger
        { 
            get
            {
                if (Fragments.Count == 0)
                    return null;
                return Fragments.Find(x => x is WCFLoggerFragment) as WCFLoggerFragment;
            } 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusaLogger.Logger"/> class.
        /// </summary>
        public Logger()
        {
            Fragments = new List<ILoggerFragment>();

        }

        public T GetFragment<T>() where T : ILoggerFragment
        {
            var the_fragment = Fragments.First(x => x is T);

            if (the_fragment != null)
                return (T)the_fragment;

            return default(T);
        }

        /// <summary>
        /// Adds a fragment to this logger.
        /// </summary>
        /// <param name="fragment">Fragment.</param>
        public void AddFragment(ILoggerFragment fragment)
        {
            if (fragment != null)
                Fragments.Add(fragment);
        }

        /// <summary>
        /// Adds a fragment of the provided type.
        /// </summary>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void AddFragment<T>(int minimumLogLevel = LogLevel.Debug) where T : ILoggerFragment
        {
            ILoggerFragment fragment;

            if (typeof(IConsoleLoggerFragment).IsEquivalentTo(typeof(T)))
                fragment = new ConsoleLoggerFragment();
            else if (typeof(IMongoDBLoggerFragment).IsEquivalentTo(typeof(T)))
                fragment = new MongoDBLoggerFragment();
            else if (typeof(IFileLoggerFragment).IsEquivalentTo(typeof(T)))
                fragment = new FileLoggerFragment();
            else
                fragment = new WCFLoggerFragment();

            fragment.SetMinimumLogLevel(minimumLogLevel);
            Fragments.Add(fragment);
        }

        /// <summary>
        /// Log the specified message using all the registered loggers.
        /// </summary>
        public void Log(int level, string message)
        {
            foreach (ILoggerFragment a in Fragments)
            {
                if (a != null)
                    a.Log(level, message);
            }
        }

        /// <summary>
        /// Gets the fragments.
        /// </summary>
        /// <returns>The fragments.</returns>
        public IEnumerable<ILoggerFragment> GetFragments()
        {
            return Fragments;
        }

        public void SetMinimumLogLevel<T>(int level)
        {
            if (!(typeof(ILoggerFragment).IsAssignableFrom(typeof(T))))
                throw new Exception("Provided type " + typeof(T).Name + " is not a valid ILoggerFragment object.");

            if (Fragments.Count <= 0)
                return;
            
            Fragments.First(x => x is T).SetMinimumLogLevel(level);
        }

        public void SetMinimumLogLevel(int level)
        {
            foreach (ILoggerFragment fragment in Fragments)
                fragment.SetMinimumLogLevel(level);
        }

        /// <summary>
        /// Sets the color to be used for the next console log.
        /// </summary>
        /// <param name="BackgroundColor">Background color.</param>
        /// <param name="ForegroundColor">Foreground color.</param>
        public void SetColorForNextConsoleLog(ConsoleColor BackgroundColor, ConsoleColor ForegroundColor)
        {
            if (ConsoleLogger == null)
                return;

            ConsoleLogger.SetColorForNextLog(BackgroundColor, ForegroundColor);
        }
    }
}

