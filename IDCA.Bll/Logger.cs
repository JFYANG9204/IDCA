
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

    public class ExceptionMessages
    {
        // MDM 部分
        public const string MDMFieldIsEmpty = "MDM文档载入错误，未载入Fields集合。";
        // Table 部分
        public const string TableFieldIsNotSetted = "未设定Table对象的Field信息，无法载入MDM文档Field对象。";
        public const string TableFieldIsNotFound = "无法在MDM文档中找到对应名称的Field。";
        // Tamplate 部分
        public const string TemplateIsNotFind = "未找到用于{0}的函数模板。";
        // File
        public const string FileWriteError = "文本写入错误，路径：{0}";
    }

    public class Messages
    {
        public const string FileWriteSuccess = "文件：'{0}'写入完成";
    }
}
