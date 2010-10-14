/*************************************************************************
 *     This file & class is part of the Object-Oriented Optimization
 *     Toolbox (or OOOT) Project
 *     Copyright 2010 Matthew Ira Campbell, PhD.
 *
 *     OOOT is free software: you can redistribute it and/or modify
 *     it under the terms of the GNU General Public License as published by
 *     the Free Software Foundation, either version 3 of the License, or
 *     (at your option) any later version.
 *  
 *     OOOT is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU General Public License for more details.
 *  
 *     You should have received a copy of the GNU General Public License
 *     along with OOOT.  If not, see <http://www.gnu.org/licenses/>.
 *     
 *     Please find further details and contact information on OOOT
 *     at http://ooot.codeplex.com/.
 *************************************************************************/
using System;

namespace OptimizationToolbox
{
    public static class SearchIO
    {
        public static int iteration { get; set; }

        public static Boolean terminateRequest { get; set; }


        public static TimeSpan timeInterval { get; set; }

        public static int verbosity { get; set; }

        #region Outputting to sidebar Console
        public static void output(object message, int verbosityLimit)
        {
            if ((verbosityLimit <= verbosity) && (message != null))
                Console.WriteLine(message);
        }
        public static void output(object message0)
        {
            if (message0 != null)
                Console.WriteLine(message0);
        }
        public static void output(object message0, object message1)
        { output(message0, message1, message1, message1, message1); }
        public static void output(object message0, object message1, object message2)
        { output(message0, message1, message2, message2, message2); }
        public static void output(object message1, object message2, object message3, object message4)
        { output(message1, message2, message3, message4, message4); }
        public static void output(object message1, object message2, object message3, object message4,
            object message5)
        {
            if (verbosity == 0) return;
            object message = null;
            switch (verbosity)
            {
                case 1: message = message1; break;
                case 2: message = message2; break;
                case 3: message = message3; break;
                case 4: message = message4; break;
                default: message = message5; break;
            }
            if ((message != null) && (message.ToString() != ""))
                Console.WriteLine(message);
        }
        #endregion

    }
}