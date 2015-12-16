using System.Xml.Serialization;
using NLog.Config;
using NLog;

namespace MusaLogger
{
	/// <summary>
	/// The abstract class that defines the behaviour of the logger
	/// </summary>
	public abstract class Logger
	{
		[XmlElement("MinimumLogLevel")]
		public int MinimumLogLevel { get; set; }

		/// <summary>
		/// Gets or sets the nlog logging configuration.
		/// </summary>
		/// <value>The configuration.</value>
		[XmlIgnore()]
		public LoggingConfiguration Configuration
		{
			get
			{
				if (conf == null)
					conf = new LoggingConfiguration ();

				return conf;
			}
			set
			{
				conf = value;
			}
		}
		[XmlIgnore()]
		private LoggingConfiguration conf;

		/// <summary>
		/// Gets the nlog logger.
		/// </summary>
		/// <value>The logger.</value>
		[XmlIgnore()]
		protected NLog.Logger logger
		{
			get { return LogManager.GetLogger (GetType ().Name); }
		}

		/// <summary>
		/// Gets the name of this logger (the class type name)
		/// </summary>
		[XmlIgnore()]
		protected string LoggerName {
			/*get {
				var m = new StackTrace ().GetFrame (3).GetMethod ();
				return m.DeclaringType.Name;
			}*/
			get
			{ 
				return GetType ().Name;
			}
		}


		protected NLog.LogLevel GetLogLevel(LogLevel level)
		{
			switch(level)
			{
			case LogLevel. Fatal:
				return NLog.LogLevel.Fatal;

			case LogLevel. Error:
				return NLog.LogLevel.Error;

			case LogLevel. Warn:
				return NLog.LogLevel.Warn;

			case LogLevel. Info:
				return NLog.LogLevel.Info;

			case LogLevel. Debug:
				return NLog.LogLevel.Debug;

			case LogLevel. Trace:
			default:
				return NLog.LogLevel.Trace;
			}
		}

		/// <summary>
		/// Log the specified message.
		/// </summary>
		public abstract void Log (LogLevel level, string message);


	}
}

