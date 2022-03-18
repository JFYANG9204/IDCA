
using System;

namespace IDCA.Bll
{

    public delegate void LogExceptionEventHandler(string message);

    public class Logger
    {

        static event LogExceptionEventHandler? Log = null;
        public static void Error(string message)
        {
            Log?.Invoke(message);
        }

        public static void Warning(string message)
        {
            Log?.Invoke(message);
        }

        public static void SetLogHandler(LogExceptionEventHandler handler)
        {
            if (Log != null)
            {
                Delegate[] delegates = Log.GetInvocationList();
                if (delegates != null)
                {
                    foreach (Delegate d in delegates)
                    {
                        Log -= (LogExceptionEventHandler)d;
                    }
                }
            }
            Log = handler;
        }

    }
}
