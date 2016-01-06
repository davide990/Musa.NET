//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  ILogger.cs
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
using System.Collections.Generic;

namespace MusaCommon
{
    public interface ILogger
    {
        void Log(int level, string message);

        /// <summary>
        /// Sets the color to be used for the next console log.
        /// </summary>
        /// <param name="bg">Background color</param>
        /// <param name="fg">Foreground color</param>
        void SetColorForNextConsoleLog(System.ConsoleColor bg, System.ConsoleColor fg);


        IEnumerable<ILoggerFragment> GetFragments();

        void AddFragment(IEnumerable<ILoggerFragment> fragments);
        void AddFragment(ILoggerFragment fragment);
    }
}

