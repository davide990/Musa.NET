using System;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using MusaLogger;
using System.Reflection;
using MusaCommon;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.Security;

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
        [XmlArrayItem("ConsoleLogger", typeof(ConsoleLoggerFragment))]
        [XmlArrayItem("FileLogger", typeof(FileLoggerFragment))]
        [XmlArrayItem("MongoDBLogger", typeof(MongoDBLoggerFragment))]
        [XmlArrayItem("WCFLogger", typeof(WCFLoggerFragment))]
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
        public MongoDBLoggerFragment MongoDBLogger
        { 
            get
            { 
                if (LoggerFragments == null)
                    return null;
                else
                    return LoggerFragments.Find(x => x is MongoDBLoggerFragment) as MongoDBLoggerFragment; 
            } 
            set
            { 
                LoggerFragments.RemoveAll(x => x is MongoDBLoggerFragment); 
                if (value is MongoDBLoggerFragment)
                    LoggerFragments.Add(value);
            }
        }

        [XmlIgnore()]
        public ConsoleLoggerFragment ConsoleLogger
        { 
            get
            { 
                if (LoggerFragments == null)
                    return null;
                else
                    return LoggerFragments.Find(x => x is ConsoleLoggerFragment) as ConsoleLoggerFragment; 
            } 
            set
            { 
                LoggerFragments.RemoveAll(x => x is MongoDBLoggerFragment); 
                if (value is ConsoleLoggerFragment)
                    LoggerFragments.Add(value);
            }
        }

        [XmlIgnore()]
        public FileLoggerFragment FileLogger
        { 
            get
            { 
                if (LoggerFragments == null)
                    return null;
                else
                    return LoggerFragments.Find(x => x is FileLoggerFragment) as FileLoggerFragment; 
            }
            set
            { 
                LoggerFragments.RemoveAll(x => x is MongoDBLoggerFragment); 
                if (value is FileLoggerFragment)
                    LoggerFragments.Add(value);
            }
        }

        [XmlIgnore()]
        public WCFLoggerFragment WCFLogger
        { 
            get { return LoggerFragments.Find(x => x is WCFLoggerFragment) as WCFLoggerFragment; } 
            set
            { 
                LoggerFragments.RemoveAll(x => x is MongoDBLoggerFragment); 
                if (value is WCFLoggerFragment)
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
        private MusaConfig()
        {
            LoggerFragments = new List<LoggerFragment>();
            Agents = new List<AgentEntry>();
            PlanLibraries = new List<Assembly>();

            MaxNumAgent = "100";
            MusaAddress = "127.0.0.1";
            MusaAddressPort = 8089;
            NetworkingEnabled = true;

        }

        #region Logger methods

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
            {
                instance.LoggerFragments.Add(new ConsoleLoggerFragment());
                return;
            }

            var logger = ModuleProvider.Get().Resolve<ILogger>();
            foreach(ILoggerFragment fragment in instance.LoggerFragments)
            {
                logger.AddFragment(fragment);
            }
        }

        #endregion Logger methods

        /// <summary>
        /// Gets the MUSA configuration.
        /// </summary>
        /// <returns>The config.</returns>
        /// <param name="addDefaultConsoleLogger">If set to <c>true</c> add the default console logger.</param>
        public static MusaConfig GetConfig(bool addDefaultConsoleLogger = true)
        {
            if (instance == null)
            {
                //Initialize a new musa configuration
                instance = new MusaConfig();

                //Initialize the logger fragments list
                instance.LoggerFragments = new List<LoggerFragment>();

                //Create a default console logger
                if (addDefaultConsoleLogger)
                    instance.LoggerFragments.Add(new ConsoleLoggerFragment());
            }
			
            return instance;
        }

        /// <summary>
        /// Replaces the invalid XML chars in a given regular expression match.
        /// </summary>
        /// <returns>The string representation of the match without the XML invalid chars.</returns>
        /// <param name="m">M.</param>
        private static string ReplaceInvalidXMLChars(Match m)
        {
            var the_string = m.ToString();
            the_string = the_string.Replace("\"", "");
            the_string = SecurityElement.Escape(the_string);
            return string.Format("\"{0}\"", the_string);
        }

        //TODO probabilmente non sarà piu necessaria dato che i caratteri invalidi non saranno piu permessi nelle formule
        //data la nuova grammatica
        /// <summary>
        /// Preprocesses the xml configuration document by replacing invalid xml character. Since parametric statements 
        /// may contains '&lt;' symbol, these are replaced with the UTF-8 corresponding symbols.
        /// </summary>
        /// <returns>The processed xml document.</returns>
        /// <param name="filename">Filename.</param>
        private static StringReader PreprocessXmlDocument(string filename)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(filename, FileMode.Open);    
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("Cannot read configuration file '" + filename + "'.\nError: " + ex);
                fs.Close();
                return null;
            }

            StreamReader reader = new StreamReader(fs);
            string[] content = reader.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            StringBuilder sb = new StringBuilder();
            foreach (string s in content)
            {
                string result = Regex.Replace(s, @"\""[^\""]*\""",
                                    new MatchEvaluator(ReplaceInvalidXMLChars));
                sb.Append(result);
                sb.Append("\n");
            }

            fs.Close();

            return new StringReader(sb.ToString());
        }

        /// <summary>
        /// Reads a MUSA configuration from file.
        /// </summary>
        public static void ReadFromFile(string filename)
        {
            // Creates an instance of the XmlSerializer class;
            // specifies the type of object to be deserialized.
            XmlSerializer serializer = new XmlSerializer(typeof(MusaConfig));

            // Declares an object variable of the type to be deserialized.
            // Uses the Deserialize method to restore the object's state 
            // with data from the XML document. */
            try
            {
                //Deserialize the XML configuration file
                //instance = (MusaConfig)serializer.Deserialize(fs);
                instance = (MusaConfig)serializer.Deserialize(PreprocessXmlDocument(filename));

                //Setup the logger
                setupLogger();

                //Read the specified plan libraries dlls
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
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Format("[MusaConfig: MaxNumAgent={0}, MusaAddress={1}, MusaAddressPort={2}, NetworkingEnabled={3}, Loggers={4}, FileName={5}]", MaxNumAgent, MusaAddress, MusaAddressPort, NetworkingEnabled, LoggerFragments, FileName);
        }
		
    }
}

