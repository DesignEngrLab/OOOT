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
using System.Diagnostics;

namespace OptimizationToolbox
{
    public static class SearchIO
    {
        private static readonly TimeSpan[] verbosityInterval = new[]
                                                                   {
                                                                       new TimeSpan(0, 0, 0, 0, 30),
                                                                       new TimeSpan(0, 0, 0, 0, 100),
                                                                       new TimeSpan(0, 0, 0, 0, 300),
                                                                       new TimeSpan(0, 0, 0, 1, 0),
                                                                       new TimeSpan(0, 0, 0, 3, 0),
                                                                       new TimeSpan(0, 0, 0, 10, 0),
                                                                       new TimeSpan(0, 0, 0, 30, 0)
                                                                   };

        private static Stopwatch timer = Stopwatch.StartNew();

        public static int verbosity { get; set; }

        #region Outputting to sidebar Console

        public static Boolean output(object message, int verbosityLimit = 1)
        {
            if ((message == null) || (message.ToString() == "")) return true;
            if (verbosityLimit == 0)
            {
                Console.WriteLine(message.ToString());
                timer.Reset();
                timer.Start();
                return true;
            }
            var index = verbosityLimit - verbosity + 2;
            if ((index < 0) || ((index < 7) && (verbosityInterval[index] <= timer.Elapsed)))
            {
                Console.WriteLine(message.ToString());
                timer.Reset();
                timer.Start();
                return true;
            }
            return false;
        }
        public static void output(params object[] list)
        {
            var index = list.Length;
            while (index-- > 0 && !output(list[index], index))
            {
            }
        }

        #endregion
    }
}