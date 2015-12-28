using System.Xml.Serialization;
using System.ServiceModel;
using System;
using MusaLogger;
using NLog.Config;
using NLog;

namespace MusaLogger
{
	public class WCFLogger : Logger
	{
		[XmlAttribute("Enabled")]
		public bool Enabled { get; set; }

		[XmlAttribute ("EndpointAddress")]
		public string EndpointAddress { get; set; }

		[XmlIgnore ()]
		private bool configured;

		public void configure()
		{
			// Create targets and add them to the configuration 
			var wcfTarget = new WCFTarget();
			wcfTarget.EndpointAddress = EndpointAddress;
			wcfTarget.Name = LoggerName;

			// Set target properties 
			wcfTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";

			// Define rules
			var rule1 = new LoggingRule(GetType().Name, GetLogLevel(LogLevel.Debug), wcfTarget);

			Configuration.LoggingRules.Add(rule1);
			Configuration.AddTarget(LoggerName, wcfTarget);
			Configuration.Reload ();

			// Activate the configuration
			LogManager.Configuration = Configuration;
		}	

        public override void Log (int level, string message)
		{
			if (!configured) 
			{
				//Configure at the first log, since FileName and Layout are not avaible until the constructor 
				//method ends
				configured = true;
				configure ();
			}

			if (Enabled && (int)level >= MinimumLogLevel)
			{
				try
				{
					logger.Log(GetLogLevel(level), message);
				}
				catch(EndpointNotFoundException e)
				{
					Console.WriteLine ("An error occurred while logging using WCF service.\n" + e.InnerException);
				}
			}
		}


	}
}

