using System.Collections.Generic;
using MusaCommon;
using System;

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

        public MongoDBLogger MongoDBLogger
        { 
            get
            { 
                if (Fragments == null)
                    return null;
                else
                    return Fragments.Find(x => x is MongoDBLogger) as MongoDBLogger; 
            } 
        }

        public ConsoleLogger ConsoleLogger
        { 
            get
            { 
                if (Fragments == null)
                    return null;
                else
                    return Fragments.Find(x => x is ConsoleLogger) as ConsoleLogger; 
            } 
        }

        public FileLogger FileLogger
        { 
            get
            { 
                if (Fragments == null)
                    return null;
                else
                    return Fragments.Find(x => x is FileLogger) as FileLogger; 
            }
        }

        public WCFLogger WCFLogger
        { 
            get { return Fragments.Find(x => x is WCFLogger) as WCFLogger; } 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusaLogger.Logger"/> class.
        /// </summary>
        public Logger()
        {
            Fragments = new List<ILoggerFragment>();
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
        /// Adds a set of fragments to this logger.
        /// </summary>
        /// <param name="fragments">Fragments.</param>
        public void AddFragment(IEnumerable<ILoggerFragment> fragments)
        {
            Fragments.AddRange(fragments);
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

