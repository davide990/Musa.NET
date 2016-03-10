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

