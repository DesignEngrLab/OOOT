using System;
using System.Threading;
using System.Collections;

namespace OptimizationToolbox
{
    public static class SearchIO
    {
        public static int processNum
        {
            get
            {
                int namelength = Thread.CurrentThread.Name.Length;
                return int.Parse(Thread.CurrentThread.Name.Substring(1, namelength - 3));
            }
        }

        #region Iteration Handling
        static int defaultIteration = 0;
        static Hashtable iterations = new Hashtable();
        public static int iteration
        {
            set
            {
                string searchThreadName = Thread.CurrentThread.Name;
                if (searchThreadName == null) defaultIteration = value;
                else if (iterations.ContainsKey(searchThreadName))
                    iterations[searchThreadName] = value;
                else iterations.Add(searchThreadName, value);
            }
            get
            {
                if (Thread.CurrentThread.Name == null) return defaultIteration;
                else return getIteration(Thread.CurrentThread.Name);
            }
        }
        public static int getIteration(string threadName)
        {
            if (iterations.ContainsKey(threadName))
                return (int)iterations[threadName];
            else return defaultIteration;
        }
        #endregion


        #region Termination Request Handling
        static Hashtable termRequests = new Hashtable();

        public static void setTerminationRequest(string threadName)
        {
            if (termRequests.ContainsKey(threadName))
                termRequests[threadName] = true;
            else termRequests.Add(threadName, true);
        }
        public static Boolean terminateRequest
        {
            get
            {
                string searchThreadName = Thread.CurrentThread.Name;
                if (searchThreadName == null) return false;
                else if (termRequests.ContainsKey(searchThreadName))
                    return (Boolean)termRequests[searchThreadName];
                else return false;
            }
        }
        #endregion

        #region Time Interval Handling
        static Hashtable timeIntervals = new Hashtable();
        static TimeSpan zeroTimeInterval = new TimeSpan(0);

        public static void setTimeInterval(string threadName, TimeSpan value)
        {
            if (timeIntervals.ContainsKey(threadName))
                timeIntervals[threadName] = value;
            else timeIntervals.Add(threadName, value);
        }
        public static TimeSpan timeInterval
        {
            get
            {
                if (Thread.CurrentThread.Name == null) return zeroTimeInterval;
                else return getTimeInterval(Thread.CurrentThread.Name);
            }
        }
        public static TimeSpan getTimeInterval(string threadName)
        {
            if (timeIntervals.ContainsKey(threadName))
                return (TimeSpan)timeIntervals[threadName];
            else return zeroTimeInterval;
        }
        #endregion

        #region Verbosity Handling
        public static int defaultVerbosity;
        static Hashtable verbosities = new Hashtable();

        public static void setVerbosity(string threadName, int value)
        {
            if (verbosities.ContainsKey(threadName))
                verbosities[threadName] = value;
            else verbosities.Add(threadName, value);
        }
        private static int verbosity
        {
            get
            {
                if (Thread.CurrentThread.Name == null) return defaultVerbosity;
                else return getVerbosity(Thread.CurrentThread.Name);
            }
        }
        public static int getVerbosity(string threadName)
        {
            if (verbosities.ContainsKey(threadName))
                return (int)verbosities[threadName];
            else return defaultVerbosity;
        }
        #endregion

        #region Outputting to sidebar Console
        /* calling SearchIO.output will output the string, message, to the 
         * text display on the right of GraphSynth, but ONLY if the verbosity (see
         * below) is greater than or equal to your specified limit for this message.
         * the verbosity limit must be 0, 1, 2, 3, or 4. */
        public static void output(object message, int verbosityLimit)
        {
            if ((verbosityLimit <= verbosity) && (message != null))
                Console.WriteLine(message);
        }
        /* the approach to SearchIO is to send multiple strings - one for each verbosity level.
         * if less than five strings are sent then the higher levels will output the last string
         * sent. For levels that you do not want to output anything, set the string to null or "" */
        public static void output(object message0)
        {
            if (message0 != null)
                Console.WriteLine(message0);
        }
        public static void output(object message0, object message1)
        { output(message0, message1, message1, message1, message1); }
        public static void output(object message0, object message1, object message2)
        { output(message0, message1, message2, message2, message2); }
        public static void output(object message0, object message1, object message2, object message3)
        { output(message0, message1, message2, message3, message3); }
        public static void output(object message0, object message1, object message2, object message3,
            object message4)
        {
            object message = null;
            switch (verbosity)
            {
                case 0: message = message0; break;
                case 1: message = message1; break;
                case 2: message = message2; break;
                case 3: message = message3; break;
                case 4: message = message4; break;
            }
            if ((message != null) && (message.ToString() != ""))
                Console.WriteLine(message);
        }
        #endregion

    }
}