using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using MetroLog;

namespace SharedCode
{
    public class LogHelper
    {
        private ILogger _log;

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
