using System.Xml.Serialization;
using System;
using NLog;
using NLog.Config;
using NLog.Mongo;
using MusaCommon;

namespace MusaLogger
{
	/// <summary>
	/// Mongo DB logger.
	/// </summary>
	public sealed class MongoDBLoggerFragment : LoggerFragment, IMongoDBLoggerFragment
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="MusaConfig.MongoDBLogger"/> is enabled.
		/// </summary>
		/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		[XmlAttribute ("Enabled")]
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the mongoDB ip address.
		/// </summary>
		/// <value>The mongoDB address.</value>
		[XmlAttribute ("MongoDBAddress")]
		public string MongoDBAddress { get; set; }

		/// <summary>
		/// Gets or sets the mongoDB port.
		/// </summary>
		/// <value>The mongoDB port.</value>
		[XmlAttribute ("MongoDBPort")]
		public int MongoDBPort { get; set; }

		/// <summary>
		/// Gets or sets the name of the mongoDB collection name.
		/// </summary>
		/// <value>The name of the mongoDB collection.</value>
		[XmlAttribute ("MongoDBCollectionName")]
		public string MongoDBCollectionName { get; set; }

		/// <summary>
		/// Gets or sets the mongoDB user.
		/// </summary>
		/// <value>The mongoDB user.</value>
		[XmlAttribute ("MongoDBUser")]
		public string MongoDBUser { get; set; }

		/// <summary>
		/// Gets or sets the mongoDB user password.
		/// </summary>
		/// <value>The mongo DB pass.</value>
		[XmlAttribute ("MongoDBPass")]
		public string MongoDBPass { get; set; }

		[XmlIgnore ()]
		private bool configured;

		/// <summary>
		/// Initializes a new instance of the <see cref="MusaConfig.MongoDBLogger"/> class.
		/// </summary>
		public MongoDBLoggerFragment ()
		{
			configured = false;
            Enabled = true;
		}

		/// <summary>
		/// Configure this logger
		/// </summary>
		private void configure ()
		{
			// Create a new MongoDB nlog target
			var mongoTarget = new NLog.Mongo.MongoTarget ();

			// set the attributes
			mongoTarget.CollectionName = MongoDBCollectionName;
			mongoTarget.ConnectionString = String.Format ("mongodb://{0}:{1}@{2}:{3}/musa_log", MongoDBUser, MongoDBPass, MongoDBAddress, MongoDBPort);

			// Define rules
            var rule1 = new LoggingRule (LoggerName, GetLogLevel(MinimumLogLevel), mongoTarget);

			Configuration.AddTarget (LoggerName, mongoTarget);
			Configuration.LoggingRules.Add (rule1);
			Configuration.Reload ();

			// Activate the configuration
			LogManager.Configuration = Configuration;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="MusaConfig.MongoDBLogger"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="MusaConfig.MongoDBLogger"/>.</returns>
		public override string ToString ()
		{
			return string.Format ("[MongoDBLogger: Enabled={0}, MongoDBAddress={1}, MongoDBPort={2}, MongoDBCollectionName={3}, MongoDBUser={4}, MongoDBPass={5}]", Enabled, MongoDBAddress, MongoDBPort, MongoDBCollectionName, MongoDBUser, MongoDBPass);
		}

		/// <summary>
		/// Log the specified message.
		/// </summary>
        public void Log (int level, string message)
		{
			if (!configured) 
			{
				//Configure at the first log, since FileName and Layout are not avaible until the constructor 
				//method ends
				configured = true;
				configure ();
			}

			if (Enabled && (int)level >= MinimumLogLevel)
				logger.Log (GetLogLevel(level), message);
		}

        #region IMongoDBLoggerFragment methods

        public string GetMongoDBAddress()
        {
            return MongoDBAddress;
        }

        public void SetMongoDBAddress(string mongodbaddress)
        {
            MongoDBAddress = mongodbaddress;
        }

        public int GetMongoDBPort()
        {
            return MongoDBPort;
        }

        public void SetMongoDBPort(int mongodbport)
        {
            MongoDBPort = mongodbport;
        }

        public string GetMongoDBCollectionName()
        {
            return MongoDBCollectionName;
        }

        public void SetMongoDBCollectionName(string mongodbcollectionname)
        {
            MongoDBCollectionName = mongodbcollectionname;
        }

        public string GetMongoDBUser()
        {
            return MongoDBUser;
        }

        public void SetMongoDBUser(string mongodbuser)
        {
            MongoDBUser = mongodbuser;
        }

        public string GetMongoDBPass()
        {
            return MongoDBPass;
        }

        public void SetMongoDBPass(string mongodbpass)
        {
            MongoDBPass = mongodbpass;
        }



        #endregion

        public void SetMinimumLogLevel(int level)
        {
            MinimumLogLevel = level;
        }

	}
}

