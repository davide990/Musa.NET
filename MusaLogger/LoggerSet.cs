using System.Collections.Generic;
using System.Linq;

namespace MusaLogger
{
    /// <summary>
    /// A convenient class used to collect all the registered loggers.
    /// </summary>
    public class LoggerSet
    {
        public List<Logger> Loggers
        {
            get;
            private set;
        }

        public MongoDBLogger MongoDBLogger
        { 
            get
            { 
                if (Loggers == null)
                    return null;
                else
                    return Loggers.Find(x => x is MongoDBLogger) as MongoDBLogger; 
            } 
        }

        public ConsoleLogger ConsoleLogger
        { 
            get
            { 
                if (Loggers == null)
                    return null;
                else
                    return Loggers.Find(x => x is ConsoleLogger) as ConsoleLogger; 
            } 
        }

        
        public FileLogger FileLogger
        { 
            get
            { 
                if (Loggers == null)
                    return null;
                else
                    return Loggers.Find(x => x is FileLogger) as FileLogger; 
            }
        }

        
        public WCFLogger WCFLogger
        { 
            get { return Loggers.Find(x => x is WCFLogger) as WCFLogger; } 
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="MusaLogger.LoggerSet"/> class.
        /// </summary>
        /// <param name="loggers">The logger set found within the <see cref="MusaConfiguration.MusaConfig"/>
        /// configuration.</param>
        public LoggerSet(List<Logger> loggers)
        {
            Loggers = loggers.Where(x => x != null).ToList();
        }

        /*public LoggerSet()
		{
			loggers = new List<Logger> ();
		}*/

        /// <summary>
        /// Log the specified message using all the registered loggers.
        /// </summary>
        public void Log(int level, string message)
        {
            foreach (Logger a in Loggers)
            {
                if (a != null)
                    a.Log(level, message);
            }
        }

        /// <summary>
        /// Gets the avaible registered loggers.
        /// </summary>
        public List<Logger> GetLoggers()
        {
            return Loggers;
        }
    }
}

