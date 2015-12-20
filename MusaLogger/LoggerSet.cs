using System.Collections.Generic;
using System.Linq;

namespace MusaLogger
{
	/// <summary>
	/// A convenient class used to collect all the registered loggers.
	/// </summary>
	public class LoggerSet
	{
		public List<Logger> loggers 
		{
			get;
			private set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MusaLogger.LoggerSet"/> class.
		/// </summary>
		/// <param name="loggers">The logger set found within the <see cref="MusaConfiguration.MusaConfig"/>
		/// configuration.</param>
		public LoggerSet(List<Logger> loggers)
		{
			this.loggers = loggers.Where(x => x != null).ToList();
		}

		/// <summary>
		/// Log the specified message using all the registered loggers.
		/// </summary>
		public void Log(LogLevel level, string message)
		{
			foreach (Logger a in loggers) 
			{
				if (a != null)
				{
					a.Log (level, message);

				}
					
			}
		}

		/// <summary>
		/// Gets the avaible registered loggers.
		/// </summary>
		public List<Logger> GetLoggers()
		{
			return loggers;
		}

	}
}

