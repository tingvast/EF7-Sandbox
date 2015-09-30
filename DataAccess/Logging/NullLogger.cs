using Microsoft.Framework.Logging;
using System;

namespace DataAccess.Logging
{
    public class NullLogger : ILogger
    {
        private NullLogger()
        {
        }

        private static NullLogger _instance = null;
        private static object syncRoot = new object();

        internal static NullLogger Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new NullLogger();
                    }
                }

                return _instance;
            }
        }

        public IDisposable BeginScopeImpl(object state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            // Do nothing!
        }
    }
}