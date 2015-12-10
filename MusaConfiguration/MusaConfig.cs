using System;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;

namespace MusaConfig
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
				if (_mongoDBLogger == null)
					throw new Exception ("An exception occurred. Ensure to read the configuration using ReadFromFile(...) method.\n");

				return _mongoDBLogger;
			}
			private set
			{
				_mongoDBLogger = value;
			}
		}
		[XmlIgnore()]
		private MongoDBLogger _mongoDBLogger;

		[XmlIgnore()]
		public ConsoleLogger ConsoleLogger 
		{
			get
			{
				if (_consoleLogger == null)
					throw new Exception ("An exception occurred. Ensure to read the configuration using ReadFromFile(...) method.\n");

				return _consoleLogger;
			}
			private set
			{
				_consoleLogger = value;
			}
		}
		[XmlIgnore()]
		private ConsoleLogger _consoleLogger;

		[XmlIgnore()]
		public FileLogger FileLogger
		{
			get
			{
				if (_fileLogger == null)
					throw new Exception ("An exception occurred. Ensure to read the configuration using ReadFromFile(...) method.\n");

				return _fileLogger;
			}
			private set
			{
				_fileLogger = value;
			}
		}
		[XmlIgnore()]
		private FileLogger _fileLogger;

		[XmlIgnore()]
		public WCFLogger WCFLogger 
		{
			get
			{
				if (_wcfLogger == null)
					throw new Exception ("An exception occurred. Ensure to read the configuration using ReadFromFile(...) method.\n");

				return _wcfLogger;
			}
			private set
			{
				_wcfLogger = value;
			}
		}
		[XmlIgnore()]
		private WCFLogger _wcfLogger;

		/// <summary>
		/// Reads a MUSA configuration from file.
		/// </summary>
		public static MusaConfig ReadFromFile(string filename)
		{
			// Creates an instance of the XmlSerializer class;
			// specifies the type of object to be deserialized.
			XmlSerializer serializer = new XmlSerializer (typeof(MusaConfig));

			// A FileStream is needed to read the XML document.
			FileStream fs = new FileStream(filename, FileMode.Open);

			// Declares an object variable of the type to be deserialized.
			MusaConfig po;

			// Uses the Deserialize method to restore the object's state 
			// with data from the XML document. */
			po = (MusaConfig) serializer.Deserialize(fs);


			foreach(MusaLogger l in po.Loggers)
			{
				if (l is WCFLogger)
					po.WCFLogger = l as WCFLogger;
				else if (l is MongoDBLogger)
					po.MongoDBLogger = l as MongoDBLogger;
				else if (l is FileLogger)
					po.FileLogger = l as FileLogger;
				else
					po.ConsoleLogger = l as ConsoleLogger;
			}

			return po;
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
			writer.Close();
		}

		public override string ToString ()
		{
			return string.Format ("[MusaConfig: MaxNumAgent={0}, MusaAddress={1}, MusaAddressPort={2}, NetworkingEnabled={3}, Loggers={4}, FileName={5}]", MaxNumAgent, MusaAddress, MusaAddressPort, NetworkingEnabled, Loggers, FileName);
		}
		
	}
}

