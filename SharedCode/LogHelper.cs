using System.Runtime.CompilerServices;

using MetroLog;

namespace SharedCode
{
    public class LogHelper
    {
        private readonly ILogger _log;

        public LogHelper()
        { }

        public LogHelper(ILogger log)
        {
            _log = log;
        }

        // Log
        public void Log(LogLevel logLevel, string message,
                [CallerMemberName] string memberName = "",
                [CallerFilePath] string sourceFilePath = "",
                [CallerLineNumber] int sourceLineNumber = 0)
        {
            // Convert \n to \r            
            //for (int index = 0; index < message.Length; index++)
            //{
            //    if (index + 1 > message.Length - 1)
            //        break;

            //    if (message[index + 1] == '\n' && message[index] != '\r')
            //    {
            //        //message = message.Insert(index, "\r");
            //        //index++;
            //        message = message.Replace('\n', '\r');
            //    }
            //    else
            //    {
            //        break;
            //    }
            //}
            // Remove trailing characters that results in extra line feeds
            char[] charsToTrim = { '\r', '\n' };
            message = message.TrimEnd(charsToTrim);
            switch (logLevel)
            {
                case LogLevel.Trace:
                    _log.Trace($" {sourceLineNumber} | {message}");
                    break;
                case LogLevel.Debug:
                    _log.Debug($" {sourceLineNumber} | {message}");
                    break;
                case LogLevel.Info:
                    _log.Info($" {sourceLineNumber} | {message}");
                    break;
                case LogLevel.Warn:
                    _log.Warn($" {sourceLineNumber} | {message}");
                    break;
                case LogLevel.Error:
                    _log.Error($" {sourceLineNumber} | {message}");
                    break;
                case LogLevel.Fatal:
                    _log.Fatal($" {sourceLineNumber} | {message}");
                    break;
            }
        }
    }
}
