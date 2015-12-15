using System.Xml.Serialization;
using NLog.Config;
using NLog.Targets;
using NLog;

namespace MusaLogger
{
	public class ConsoleLogger : Logger
	{
		[XmlAttribute("Enabled")]
		public bool Enabled { get; set; }

		public ConsoleLogger()
		{
			configure ();
		}

		private void configure()
		{
			// Create targets and add them to the configuration 
			var consoleTarget = new ColoredConsoleTarget();
			Configuration.AddTarget(GetType().Name, consoleTarget);

			// Set target properties 
			consoleTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";

			// Define rules
			var rule1 = new LoggingRule("*", GetLogLevel(LogLevel.Debug), consoleTarget);
			Configuration.LoggingRules.Add(rule1);

			// Activate the configuration
			LogManager.Configuration = Configuration;
		}

		public override void Log (LogLevel level, string message)
		{
			if (Enabled)
				logger.Log (GetLogLevel(level), message);
		}

		public override string ToString ()
		{
			return string.Format ("[ConsoleLogger: Enabled={0}]", Enabled);
		}
	}
}

