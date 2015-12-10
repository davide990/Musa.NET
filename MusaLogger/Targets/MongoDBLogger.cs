using System.Xml.Serialization;
using System;
using NLog;
using NLog.Config;
using NLog.MongoDB;

namespace MusaConfig
{
	/// <summary>
	/// Mongo DB logger.
	/// </summary>
	public sealed class MongoDBLogger : MusaLogger
	{
		[XmlAttribute ("Enabled")]
		public bool Enabled { get; set; }

		[XmlAttribute ("MongoDBAddress")]
		public string MongoDBAddress { get; set; }

		[XmlAttribute ("MongoDBPort")]
		public int MongoDBPort { get; set; }

		[XmlAttribute ("MongoDBCollectionName")]
		public string MongoDBCollectionName { get; set; }

		[XmlAttribute ("MongoDBUser")]
		public string MongoDBUser { get; set; }

		[XmlAttribute ("MongoDBPass")]
		public string MongoDBPass { get; set; }

		[XmlIgnore ()]
		private bool configured;

		public MongoDBLogger ()
		{
			configured = false;
		}


		public void configure ()
		{
			var mongoTarget = new MongoDBTarget ();
			Configuration.AddTarget ("mongodb", mongoTarget);

			mongoTarget.CollectionName = MongoDBCollectionName;
			mongoTarget.ConnectionString = String.Format ("mongodb://{0}:{1}@{2}:{3}/musa_log?connectTimeoutMS=10000", MongoDBUser, MongoDBPass, MongoDBAddress, MongoDBPort);

			// Define rules
			var rule1 = new LoggingRule ("*", LogLevel.Debug, mongoTarget);
			Configuration.LoggingRules.Add (rule1);

			// Activate the configuration
			LogManager.Configuration = Configuration;
		}


		public override string ToString ()
		{
			return string.Format ("[MongoDBLogger: Enabled={0}, MongoDBAddress={1}, MongoDBPort={2}, MongoDBCollectionName={3}, MongoDBUser={4}, MongoDBPass={5}]", Enabled, MongoDBAddress, MongoDBPort, MongoDBCollectionName, MongoDBUser, MongoDBPass);
		}

		public override void Log (LogLevel level, string message)
		{
			if (!configured) {
				//Configure at the first log, since FileName and Layout are not avaible until the constructor 
				//method ends
				configured = true;
				configure ();
			}

			if (Enabled)
				Logger.Log (level, message);
		}
	}
}

