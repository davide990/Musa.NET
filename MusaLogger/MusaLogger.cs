using System.Xml.Serialization;
using NLog.Config;
using NLog;
using System.Diagnostics;

namespace MusaConfig
{
	/// <summary>
	/// The abstract class that defines the behaviour of the logger
	/// </summary>
	public abstract class MusaLogger
	{
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
		protected Logger Logger
		{
			get { return LogManager.GetLogger (GetType ().Name); }
		}

		/// <summary>
		/// Gets the name of this logger (the class type name)
		/// </summary>
		[XmlIgnore()]
		protected string LoggerName
		{
			get{
				var m = new StackTrace().GetFrame(3).GetMethod();
				return m.DeclaringType.Name;
			}
		}

		/// <summary>
		/// Log the specified message.
		/// </summary>
		public abstract void Log (LogLevel level, string message);


	}
}

