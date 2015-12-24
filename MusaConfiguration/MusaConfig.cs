using System;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using MusaLogger;

namespace MusaConfiguration
{
	[XmlRoot("MusaConfiguration")]
	public sealed class MusaConfig
	{
		#region XML elements
		[XmlElement("MaxNumAgent")]
		public string MaxNumAgent { get; set; }

		[XmlElement("MusaAddress")]
		public string MusaAddress { get; set; }

		[XmlElement("MusaAddressPort")]
		public int MusaAddressPort { get; set; }

		[XmlElement("NetworkingEnabled")]
		public bool NetworkingEnabled { get; set; }
	
		[XmlElement("MinimumLogLevel")]
		public int MinimumLogLevel { get; set; }

		[XmlArray("Logging")]
		[XmlArrayItem("ConsoleLogger", typeof(ConsoleLogger))]
		[XmlArrayItem("FileLogger", typeof(FileLogger))]
		[XmlArrayItem("MongoDBLogger", typeof(MongoDBLogger))]
		[XmlArrayItem("WCFLogger", typeof(WCFLogger))]
		public List<Logger> Loggers { get; set;	}
		#endregion XML elements

		#region Properties

		/// <summary>
		/// Gets or sets the name of the MUSA configuration file this object is related to.
		/// </summary>
		[XmlIgnore()]
		public string FileName 
		{
			get
			{
				if (String.IsNullOrEmpty (_fname))
					throw new Exception ("An exception occurred. Ensure to read the configuration using ReadFromFile(...) method.\n");

				return _fname;
			}
			private set
			{
				_fname = value;
			}
		}
		private string _fname;

		[XmlIgnore()]
		public MongoDBLogger MongoDBLogger 
		{ 
			get 
			{ 
				if (Loggers == null)
					return null;
				else
					return Loggers.Find (x => x is MongoDBLogger) as MongoDBLogger; 
			} 
			set 
			{ 
				Loggers.RemoveAll (x => x is MongoDBLogger); 
				if (value is MongoDBLogger)
					Loggers.Add (value);
			}
		}

		[XmlIgnore()]
		public ConsoleLogger ConsoleLogger 
		{ 
			get 
			{ 
				if (Loggers == null)
					return null;
				else
					return Loggers.Find (x => x is ConsoleLogger) as ConsoleLogger; 
			} 
			set 
			{ 
				Loggers.RemoveAll (x => x is MongoDBLogger); 
				if (value is ConsoleLogger)
					Loggers.Add (value);
			}
		}

		[XmlIgnore()]
		public FileLogger FileLogger 
		{ 
			get 
			{ 
				if (Loggers == null)
					return null;
				else
					return Loggers.Find (x => x is FileLogger) as FileLogger; 
			}
			set 
			{ 
				Loggers.RemoveAll (x => x is MongoDBLogger); 
				if (value is FileLogger)
					Loggers.Add (value);
			}
		}

		[XmlIgnore()]
		public WCFLogger WCFLogger 
		{ 
			get { return Loggers.Find (x => x is WCFLogger) as WCFLogger; } 
			set 
			{ 
				Loggers.RemoveAll (x => x is MongoDBLogger); 
				if (value is WCFLogger)
					Loggers.Add (value);
			}
		}

		/// <summary>
		/// A boolean value indicating wheter this configuration have any logger set. If not, the current MUSA instance
		/// will redirect its output to console.
		/// </summary>
		[XmlIgnore()]
		public bool HasLoggerConfigured 
		{
			get 
			{
				if (Loggers == null)
					return false;
				else if (Loggers.Count <= 0)
					return false;

				return true;
			}
		}

		#endregion

		/// <summary>
		/// The unique configuration instance for this MUSA environment.
		/// </summary>
		[XmlIgnore()]
		private static MusaConfig instance;

		/// <summary>
		/// The set of loggers registered within this configuration.
		/// </summary>
		[XmlIgnore()]
		private static LoggerSet loggerSet;

		public MusaConfig()
		{
			MaxNumAgent = "100";
			MusaAddress = "127.0.0.1";
			MusaAddressPort = 8089;
			NetworkingEnabled = true;
			MinimumLogLevel = 0;

		}

		/// <summary>
		/// Gets the logger set. This class contains all the avaible registered loggers.
		/// </summary>
		/// <returns>The logger set.</returns>
		public static LoggerSet GetLoggerSet()
		{
			if (loggerSet != null)
				return loggerSet;

			//If no logger has been found, create a "default" console logger
			if (instance.Loggers.Count <= 0)
				instance.Loggers.Add (new ConsoleLogger ());

			loggerSet = new LoggerSet (instance.Loggers);
			SetMinimumLogLevel ();

			return loggerSet;
		}

		private static void SetMinimumLogLevel()
		{
			foreach (Logger l in instance.Loggers)
				l.MinimumLogLevel = instance.MinimumLogLevel;
		}

		/// <summary>
		/// Gets the MUSA configuration.
		/// </summary>
		public static MusaConfig GetConfig()
		{
			if(instance == null)
			{
				instance = new MusaConfig ();
				instance.Loggers = new List<Logger> (){ new ConsoleLogger () };
			}
			
			return instance;
		}

		/// <summary>
		/// Reads a MUSA configuration from file.
		/// </summary>
		public static void ReadFromFile(string filename)
		{
			// Creates an instance of the XmlSerializer class;
			// specifies the type of object to be deserialized.
			XmlSerializer serializer = new XmlSerializer (typeof(MusaConfig));

			// A FileStream is needed to read the XML document.
			FileStream fs = new FileStream(filename, FileMode.Open);

			// Declares an object variable of the type to be deserialized.

			// Uses the Deserialize method to restore the object's state 
			// with data from the XML document. */
			instance = (MusaConfig) serializer.Deserialize(fs);
		}

		/// <summary>
		/// Save this configuration to file.
		/// </summary>
		/// <param name="fname">The file name this configuration must be saved to.</param>
		public void Save(String fname)
		{
			// Create an instance of the XmlSerializer class;
			// specify the type of object to serialize.
			XmlSerializer serializer =  new XmlSerializer(typeof(MusaConfig));
			TextWriter writer = new StreamWriter(fname);
				
			// Serialize the MUSA configuration, and close the TextWriter.
			serializer.Serialize(writer, this);

			// Close the serializer
			writer.Close();
		}

		public override string ToString ()
		{
			return string.Format ("[MusaConfig: MaxNumAgent={0}, MusaAddress={1}, MusaAddressPort={2}, NetworkingEnabled={3}, Loggers={4}, FileName={5}]", MaxNumAgent, MusaAddress, MusaAddressPort, NetworkingEnabled, Loggers, FileName);
		}
		
	}
}

