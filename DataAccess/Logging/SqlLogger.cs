using Microsoft.Framework.Logging;
using System;
using System.IO;

namespace DataAccess.Logging
{
    internal class SqlLogger : ILogger
    {
        private static readonly string _logFilePath = @"C:\Temp\EFSandbox\EF7-Sandbox\DataAccessSql.log";

        public IDisposable BeginScopeImpl(object state)
        {
            //File.AppendAllText(_logFilePath, "=======================================================");

            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log(LogLevel logLevel,
            int eventId,
            object state,
            Exception exception,
            Func<object, Exception, string> formatter)
        {
            var message = string.Format(
                "\n\n--{0}\n{1}",
                DateTime.Now,
                formatter(state, exception));

            File.AppendAllText(_logFilePath, message);
        }
    }
}