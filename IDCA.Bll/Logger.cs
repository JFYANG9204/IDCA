
using System;

namespace IDCA.Bll
{

    public delegate void LogExceptionEventHandler(string reason, string message);

    public class Logger
    {

        static event LogExceptionEventHandler? ErrorLog = null;
        static event LogExceptionEventHandler? WarningLog = null;
        static event Action<string>? MessageLog = null;

        public static void Error(string reason, string message, params string[] parameters)
        {
            ErrorLog?.Invoke(reason, string.Format(message, parameters));
        }

        public static void Warning(string reason, string message, params string[] parameters)
        {
            WarningLog?.Invoke(reason, string.Format(message, parameters));
        }

        public static void Message(string message, params string[] parameters)
        {
            MessageLog?.Invoke(string.Format(message, parameters));
        }

        public static void SetErrorLogHandler(LogExceptionEventHandler handler)
        {
            if (ErrorLog != null)
            {
                Delegate[] delegates = ErrorLog.GetInvocationList();
                if (delegates != null)
                {
                    foreach (Delegate d in delegates)
                    {
                        ErrorLog -= (LogExceptionEventHandler)d;
                    }
                }
            }
            ErrorLog += handler;
        }

    }

}
