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

        public static int verbosity { get; set; }


        #region Outputting to sidebar Console
        //private static readonly TimeSpan[] verbosityInterval = new[]
        //                                                           {
        //                                                               new TimeSpan(0, 0, 0,0, 1),
        //                                                               new TimeSpan(0, 0, 0, 0,3),
        //                                                               new TimeSpan(0, 0, 0,0, 10),
        //                                                               new TimeSpan(0, 0, 0, 0, 30),
        //                                                               new TimeSpan(0, 0, 0, 0, 100),
        //                                                               new TimeSpan(0, 0, 0, 0, 300),
        //                                                               new TimeSpan(0, 0, 0, 1, 0),
        //                                                               new TimeSpan(0, 0, 0, 3, 0),
        //                                                               new TimeSpan(0, 0, 0, 10, 0),
        //                                                               new TimeSpan(0, 0, 0, 30, 0)
        //                                                           };

        //private static readonly Stopwatch timer = Stopwatch.StartNew();


        /// <summary>
        ///  Calling SearchIO.output will output the string, message, to the 
        ///  text display on the right of GraphSynth, but ONLY if the verbosity (see
        ///  below) is greater than or equal to your specified limit for this message.
        ///  the verbosity limit must be 0, 1, 2, 3, or 4.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="verbosityLimit">The verbosity limit.</param>
        public static Boolean output(object message, int verbosityLimit = 0)
        {
            if ((verbosityLimit > verbosity)
                || (string.IsNullOrEmpty(message.ToString())))
                return false;
            Debug.WriteLine(message);
            return true;
        }
        /// <summary>
        /// Outputs the one item of the specified list corresponding to the particular verbosity.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        public static Boolean output(params object[] list)
        {
            if ((verbosity >= list.Length)
                || (string.IsNullOrEmpty(list[verbosity].ToString())))
                return false;
            Debug.WriteLine(list[verbosity]);
            return true;
        }

        ///// <summary>
        /////   This was a new and better idea but users didn't like it. It was motivated 
        /////   by an issue of sending too much to the buffer and having the program lock
        /////   up, but the problem is, people want messages to appear reliably not 
        /////   "randomly". You would see it for one iteration and not the next - and that
        /////   was frustrating.
        ///// </summary>
        ///// <param name = "message">The message.</param>
        ///// <param name = "verbosityLimit">The verbosity limit.</param>
        //public static Boolean output(object message, int verbosityLimit = 0)
        //{
        //    if ((message == null) || (message.ToString() == "")) return true;
        //    if (verbosityLimit == 0)
        //    {
        //        Debug.WriteLine(message.ToString());
        //        timer.Reset();
        //        timer.Start();
        //        return true;
        //    }
        //    var index = verbosityLimit - verbosity + 2;
        //    if ((index < 0) || ((index < 7) && (verbosityInterval[index] <= timer.Elapsed)))
        //    {
        //        Debug.WriteLine(message.ToString());
        //        timer.Reset();
        //        timer.Start();
        //        return true;
        //    }
        //    return false;
        //}

        ///// <summary>
        /////   Outputs the specified message to the output textbox -
        /////   one for each verbosity level.
        ///// </summary>
        ///// <param name = "list">The list.</param>
        //public static void output(params object[] list)
        //{
        //    var index = list.Length;
        //    while (index-- > 0 && !output(list[index], index))
        //    {
        //    }
        //}
        #endregion
    }
}