using System;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using MusaLogger;
using System.Reflection;
using MusaCommon;

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

        [XmlArray("Logging")]
        [XmlArrayItem("ConsoleLogger", typeof(ConsoleLogger))]
        [XmlArrayItem("FileLogger", typeof(FileLogger))]
        [XmlArrayItem("MongoDBLogger", typeof(MongoDBLogger))]
        [XmlArrayItem("WCFLogger", typeof(WCFLogger))]
        public List<LoggerFragment> LoggerFragments { get; set; }

        [XmlArray("Agents")]
        [XmlArrayItem("Agent", typeof(AgentEntry))]
        public List<AgentEntry> Agents { get; set; }

        /// <summary>
        /// Gets or sets the plan libraries. Each plan library is a full path to
        /// a dll containing plans
        /// </summary>
        /// <value>The plan libraries.</value>
        [XmlArray("PlanLibraries")]
        [XmlArrayItem("PlanLibrary", typeof(string))]
        public List<string> PlanLibrariesPath { get; set; }

        [XmlIgnore]
        public List<Assembly> PlanLibraries { get; set; }

        #endregion XML elements

        #region Properties

        [XmlIgnore()]
        public List<Type> DefinedPlans
        {
            get
            {
                var list = new List<Type>();

                foreach (string assembly in PlanLibrariesPath)
                {
                    Assembly planLibrary = Assembly.LoadFile(assembly);    
                    list.AddRange(planLibrary.DefinedTypes);
                }
                return list;
            }
        }

        /// <summary>
        /// Gets or sets the name of the MUSA configuration file this object is related to.
        /// </summary>
        [XmlIgnore()]
        public string FileName
        {
            get
            {
                if (String.IsNullOrEmpty(_fname))
                    throw new Exception("An exception occurred. Ensure to read the configuration using ReadFromFile(...) method.\n");

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
                if (LoggerFragments == null)
                    return null;
                else
                    return LoggerFragments.Find(x => x is MongoDBLogger) as MongoDBLogger; 
            } 
            set
            { 
                LoggerFragments.RemoveAll(x => x is MongoDBLogger); 
                if (value is MongoDBLogger)
                    LoggerFragments.Add(value);
            }
        }

        [XmlIgnore()]
        public ConsoleLogger ConsoleLogger
        { 
            get
            { 
                if (LoggerFragments == null)
                    return null;
                else
                    return LoggerFragments.Find(x => x is ConsoleLogger) as ConsoleLogger; 
            } 
            set
            { 
                LoggerFragments.RemoveAll(x => x is MongoDBLogger); 
                if (value is ConsoleLogger)
                    LoggerFragments.Add(value);
            }
        }

        [XmlIgnore()]
        public FileLogger FileLogger
        { 
            get
            { 
                if (LoggerFragments == null)
                    return null;
                else
                    return LoggerFragments.Find(x => x is FileLogger) as FileLogger; 
            }
            set
            { 
                LoggerFragments.RemoveAll(x => x is MongoDBLogger); 
                if (value is FileLogger)
                    LoggerFragments.Add(value);
            }
        }

        [XmlIgnore()]
        public WCFLogger WCFLogger
        { 
            get { return LoggerFragments.Find(x => x is WCFLogger) as WCFLogger; } 
            set
            { 
                LoggerFragments.RemoveAll(x => x is MongoDBLogger); 
                if (value is WCFLogger)
                    LoggerFragments.Add(value);
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
                if (LoggerFragments == null)
                    return false;
                else if (LoggerFragments.Count <= 0)
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
        /// Initializes a new instance of the <see cref="MusaConfiguration.MusaConfig"/> class.
        /// </summary>
        public MusaConfig()
        {
            LoggerFragments = new List<LoggerFragment>();
            Agents = new List<AgentEntry>();
            PlanLibraries = new List<Assembly>();

            MaxNumAgent = "100";
            MusaAddress = "127.0.0.1";
            MusaAddressPort = 8089;
            NetworkingEnabled = true;

        }

        public void AddLoggerFragment(object loggerFragment)
        {
			
            if (loggerFragment is LoggerFragment)
                LoggerFragments.Add(loggerFragment as LoggerFragment);	
        }

        public void AddLoggerFragment(IEnumerable<object> loggerFragments)
        {
            foreach (var v in loggerFragments)
            {
                if (v is LoggerFragment)
                    LoggerFragments.Add(v as LoggerFragment);	
            }
        }

        /// <summary>
        /// Setups the logger according to this configuration. If no logger
        /// configuration is found, a logger is created with a console fragment.
        /// </summary>
        private static void setupLogger()
        {
            //If no logger has been found, create a default console logger
            if (instance.LoggerFragments.Count <= 0)
                instance.LoggerFragments.Add(new ConsoleLogger());
            
            ModuleProvider.Get().Resolve<ILogger>().AddFragment(instance.LoggerFragments);
        }

        /// <summary>
        /// Gets the MUSA configuration.
        /// </summary>
        public static MusaConfig GetConfig()
        {
            if (instance == null)
            {
                instance = new MusaConfig();
                instance.LoggerFragments = new List<LoggerFragment>{ new ConsoleLogger() };
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
            XmlSerializer serializer = new XmlSerializer(typeof(MusaConfig));

            // A FileStream is needed to read the XML document.
            FileStream fs = null;
            try
            {
                fs = new FileStream(filename, FileMode.Open);    
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("Cannot read configuration file '" + filename + "'.\nError: " + ex);
            }
			
            // Declares an object variable of the type to be deserialized.
            // Uses the Deserialize method to restore the object's state 
            // with data from the XML document. */
            try
            {
                instance = (MusaConfig)serializer.Deserialize(fs);

                setupLogger();

                ReadExternalPlanLibraries();
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("There was an error serializing configuration" +
                    "filename '" + filename + "'.\nError: " + ex);
            }
        }

        /// <summary>
        /// Reads the external plan libraries specified within the configuration file. Each external plan
        /// library must be specified as a full path to a dll containing the MUSA library and the plan library
        /// (Use ILMerge/ILRepack to wrap multiple dll into one)
        /// </summary>
        private static void ReadExternalPlanLibraries()
        {
            foreach (string s in instance.PlanLibrariesPath)
            {
                try
                {
                    //Get the external assembly file
                    var library_assembly_file = Assembly.LoadFrom(s);

                    //Load the assembly
                    AppDomain.CurrentDomain.Load(library_assembly_file.GetName());

                    //Add the assembly to the plan library
                    instance.PlanLibraries.Add(library_assembly_file);
                }
                catch (FileNotFoundException)
                {
                    //TODO log or not log?
                    ModuleProvider.Get().Resolve<ILogger>().Log(LogLevel.Fatal, 
                        "Unable to load plan library at '" + s + "': file not found.\n");
                }
            }
        }

        /// <summary>
        /// Save this configuration to file.
        /// </summary>
        /// <param name="fname">The file name this configuration must be saved to.</param>
        public void Save(String fname)
        {
            // Create an instance of the XmlSerializer class;
            // specify the type of object to serialize.
            XmlSerializer serializer = new XmlSerializer(typeof(MusaConfig));
            TextWriter writer = new StreamWriter(fname);
				
            // Serialize the MUSA configuration, and close the TextWriter.
            serializer.Serialize(writer, this);

            // Close the serializer
            writer.Close();
        }

        //TODO implementami
        private void SaveAgentEnvironment()
        {
            //...

        }

        public override string ToString()
        {
            return string.Format("[MusaConfig: MaxNumAgent={0}, MusaAddress={1}, MusaAddressPort={2}, NetworkingEnabled={3}, Loggers={4}, FileName={5}]", MaxNumAgent, MusaAddress, MusaAddressPort, NetworkingEnabled, LoggerFragments, FileName);
        }
		
    }
}

