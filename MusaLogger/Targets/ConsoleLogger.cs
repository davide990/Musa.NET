//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  ConsoleLogger.cs
//
//  Author:
//       Davide Guastella <davide.guastella90@gmail.com>
//
//  Copyright (c) 2015 Davide Guastella
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
using NLog.Targets;
using NLog;
using System;

namespace MusaLogger
{
    public class ConsoleTarget : TargetWithLayout
    {
        protected override void Write(LogEventInfo logEvent)
        {
            string logMessage = Layout.Render(logEvent);
            Console.WriteLine(logMessage);
        }
    }

    public sealed class ConsoleLogger : Logger
    {
        [XmlAttribute("Enabled")]
        public bool Enabled { get; set; }

        [XmlIgnore()]
        private bool configured;

        public ConsoleLogger()
        {
            configured = false;
        }

        private void configure()
        {
            // Create targets and add them to the configuration 
            //var consoleTarget = new ColoredConsoleTarget();
            var consoleTarget = new ConsoleTarget();

            // Set target properties 
            consoleTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger}|${level:uppercase=true}|${message}";

            // Define rules
            //var rule1 = new LoggingRule(LoggerName, GetLogLevel(LogLevel.Trace), consoleTarget);
            var rule1 = new LoggingRule(LoggerName, GetLogLevel(MinimumLogLevel), consoleTarget);

            Configuration.AddTarget(LoggerName, consoleTarget);
            Configuration.LoggingRules.Add(rule1);
            Configuration.Reload();

            // Activate the configuration
            LogManager.Configuration = Configuration;
        }

        public override void Log(int LogLevel, string message)
        {
            if (!configured)
            {
                configured = true;
                configure();
            }
            logger.Log(GetLogLevel(LogLevel), message);
        }

        public override string ToString()
        {
            return string.Format("[ConsoleLogger: Enabled={0}]", Enabled);
        }
    }
}

