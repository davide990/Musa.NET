//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  Logger.cs
//
//  Author:
//       Davide Guastella <davide.guastella90@gmail.com>
//
//  Copyright (c) 2016 Davide Guastella
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Xml.Serialization;
using NLog.Config;
using NLog;
using MusaCommon;

namespace MusaLogger
{
    /// <summary>
    /// The abstract class that defines the behaviour of the logger fragment.
    /// </summary>
    public abstract class LoggerFragment : ILoggerFragment
    {
        [XmlAttribute("MinimumLogLevel")]
        public int MinimumLogLevel { get; set; }

        /// <summary>
        /// Gets or sets the nlog logging configuration.
        /// </summary>
        /// <value>The configuration.</value>
        [XmlIgnore()]
        public static LoggingConfiguration Configuration
        {
            get
            {
                if (conf == null)
                    conf = new LoggingConfiguration();

                return conf;
            }
            set
            {
                conf = value;
            }
        }

        [XmlIgnore()]
        private static LoggingConfiguration conf;

        /// <summary>
        /// Gets the nlog logger.
        /// </summary>
        /// <value>The logger.</value>
        [XmlIgnore()]
        protected NLog.Logger logger
        {
            get { return LogManager.GetLogger(GetType().Name); }
        }

        /// <summary>
        /// Gets the name of this fragment (the class type name)
        /// </summary>
        [XmlIgnore()]
        protected string LoggerName
        {
            get
            { 
                return GetType().Name;
            }
        }

        protected NLog.LogLevel GetLogLevel(int level)
        {
            switch (level)
            {
                case LogLevel. Fatal:
                    return NLog.LogLevel.Fatal;

                case LogLevel. Error:
                    return NLog.LogLevel.Error;

                case LogLevel. Warn:
                    return NLog.LogLevel.Warn;

                case LogLevel. Info:
                    return NLog.LogLevel.Info;

                case LogLevel. Debug:
                    return NLog.LogLevel.Debug;

                case LogLevel. Trace:
                default:
                    return NLog.LogLevel.Trace;
            }
        }

        /// <summary>
        /// Log the specified message.
        /// </summary>
        public abstract void Log(int LogLevel, string message);
    }
}

