using System;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using MusaLogger;

namespace MusaConfiguration
{
	/// <summary>
	/// Musa config.
	/// </summary>
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
	
		[XmlArray("Logging")]
		[XmlArrayItem("ConsoleLogger", typeof(ConsoleLogger))]
		[XmlArrayItem("FileLogger", typeof(FileLogger))]
		[XmlArrayItem("MongoDBLogger", typeof(MongoDBLogger))]
		[XmlArrayItem("WCFLogger", typeof(WCFLogger))]
		public List<MusaLogger> Loggers { get; set;	}
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
		public MongoDBLogger MongoDBLogger { get { return Loggers.Find (x => x is MongoDBLogger) as MongoDBLogger; } }

		[XmlIgnore()]
		public ConsoleLogger ConsoleLogger { get { return Loggers.Find (x => x is ConsoleLogger) as ConsoleLogger; } }

		[XmlIgnore()]
		public FileLogger FileLogger { get { return Loggers.Find (x => x is FileLogger) as FileLogger; } }

		[XmlIgnore()]
		public WCFLogger WCFLogger { get { return Loggers.Find (x => x is WCFLogger) as WCFLogger; } }

		#endregion

		[XmlIgnore()]
		private static MusaConfig instance;

		[XmlIgnore()]
		private static LoggerSet loggerSet;

		public static LoggerSet GetLoggerSet()
		{
			if (loggerSet != null)
				return loggerSet;

			loggerSet = new LoggerSet (instance.Loggers);
			return loggerSet;
		}

		/// <summary>
		/// Gets the MUSA configuration.
		/// </summary>
		public static MusaConfig GetConfig()
		{
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

