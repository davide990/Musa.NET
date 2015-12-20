using System.Xml.Serialization;
using NLog.Config;
using NLog.Targets;
using NLog;
using System.Diagnostics;
using System;

namespace MusaLogger
{
	public class ConsoleTarget : TargetWithLayout
	{
		protected override void Write (LogEventInfo logEvent)
		{
			string logMessage = Layout.Render(logEvent);

			if (Debugger.IsAttached)
				Trace.WriteLine (logMessage);
			else
				Console.WriteLine (logMessage);
		}
	}

	public sealed class ConsoleLogger : Logger
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
			//var consoleTarget = new ColoredConsoleTarget();
			var consoleTarget = new ConsoleTarget();

			// Set target properties 
			consoleTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";

			// Define rules
			var rule1 = new LoggingRule(LoggerName, GetLogLevel(LogLevel.Debug), consoleTarget);

			Configuration.AddTarget(LoggerName, consoleTarget);
			Configuration.LoggingRules.Add(rule1);
			Configuration.Reload ();

			// Activate the configuration
			LogManager.Configuration = Configuration;
		}

		public override void Log (LogLevel level, string message)
		{
			
			/*if (Enabled && (int)level >= MinimumLogLevel)
			{
				string s = String.Format ("{0}\t{1}\t{2}\t{3} ", DateTime.Now, LoggerName, level.ToString ().ToUpper(), message);	
				Console.WriteLine (s);
			}*/
			logger.Log (GetLogLevel(level), message);
		}

		public override string ToString ()
		{
			return string.Format ("[ConsoleLogger: Enabled={0}]", Enabled);
		}
	}
}

