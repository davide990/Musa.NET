using NLog.Targets;
using NLog.Config;
using NLog;
using System.Xml.Serialization;
using MusaCommon;

namespace MusaLogger
{
    public sealed class FileLoggerFragment : LoggerFragment, IFileLoggerFragment
    {
        [XmlAttribute("Enabled")]
        public bool Enabled { get; set; }

        [XmlAttribute("FileName")]
        public string FileName
        {
            get;
            set;
        }

        [XmlAttribute("Layout")]
        public string Layout
        {
            get;
            set;
        }

        [XmlIgnore()]
        private bool configured;

        public FileLoggerFragment()
        {
            configured = false;
        }

        void configure()
        {
            var fileTarget = new FileTarget();
            fileTarget.Name = LoggerName;

            if (string.IsNullOrEmpty(FileName))
                FileName = "${basedir}/file.txt";

            if (string.IsNullOrEmpty(Layout))
                Layout = "${longdate}\t${level:uppercase=true}\t" + LoggerName + "\t${message}";

            fileTarget.FileName = FileName;
            fileTarget.Layout = Layout;

            //var rule2 = new LoggingRule (LoggerName, GetLogLevel(LogLevel.Debug), fileTarget);
            var rule2 = new LoggingRule(LoggerName, GetLogLevel(MinimumLogLevel), fileTarget);


            Configuration.AddTarget(fileTarget);
            Configuration.LoggingRules.Add(rule2);
            Configuration.Reload();

            // Activate the configuration
            LogManager.Configuration = Configuration;
        }

        public void Log(int level, string message)
        {
            if (!configured)
            {
                //Configure at the first log, since FileName and Layout are not avaible until the constructor 
                //method ends
                configured = true;
                configure();
            }

            if (Enabled && (int)level >= MinimumLogLevel)
                logger.Log(GetLogLevel(level), message);
        }

        public void SetLayout(string layout)
        {
            Layout = layout;
        }

        public string GetLayout()
        {
            return Layout;
        }

        public void SetFilename(string filename)
        {
            FileName = filename;
        }

        public string GetFilename()
        {
            return FileName;
        }

        public void SetMinimumLogLevel(int level)
        {
            MinimumLogLevel = level;
        }
    }
}

