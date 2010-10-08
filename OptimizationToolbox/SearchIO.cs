using System;
using System.Threading;
using System.Collections;

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
                default: message = message4; break;
            }
            if ((message != null) && (message.ToString() != ""))
                Console.WriteLine(message);
        }
        #endregion

    }
}