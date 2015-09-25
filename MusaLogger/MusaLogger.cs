namespace MusaLogger
{
    public class MusaLogger
    {
        private string loggerFormat;
        private string fileName;
        private string remoteLoggerIP;
        private string remoteLoggerPort;

        /// <summary>
        /// The format of the messages logged
        /// </summary>
        public string LoggerFormat
        {
            get { return loggerFormat; }
            set { loggerFormat = value; }
        }

        /// <summary>
        /// The filename in which the logs will be appended
        /// </summary>
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        /// <summary>
        /// The IP address of a remote logger
        /// </summary>
        public string RemoteLoggerIP
        {
            get { return remoteLoggerIP; }
            set { remoteLoggerIP = value; }
        }

        /// <summary>
        /// The port of a remote logger
        /// </summary>
        public string RemoteLoggerPort
        {
            get { return remoteLoggerPort; }
            set { remoteLoggerPort = value; }
        }
    }
}
