using System.Collections.Generic;
using NLog;
using System.Linq;

namespace MusaLogger
{
	/// <summary>
	/// A convenient class used to collect all the registered loggers.
	/// </summary>
	public class LoggerSet
	{
		public List<MusaConfiguration.MusaLogger> loggers 
		{
			get;
			private set;
		}

		public LoggerSet(List<MusaConfiguration.MusaLogger> loggers)
		{
			this.loggers = loggers;
		}

		/// <summary>
		/// Log the specified message using all the registered loggers.
		/// </summary>
		public void Log(LogLevel level, string message)
		{
			foreach(MusaConfiguration.MusaLogger a in loggers)
			{
				if(a != null)
					a.Log (level, message);
			}
		}

		/// <summary>
		/// Gets the avaible registered loggers.
		/// </summary>
		public List<MusaConfiguration.MusaLogger> GetLoggers()
		{
			return loggers.Where (x => x != null).ToList();
		}

	}
}

