// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using log4net;
using log4net.Config;
using Microsoft.Framework.Logging;
using System;
using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace MyNameSpace
{
    public class TestSqlLoggerFactory : ILoggerFactory
    {
        private const string ContextName = "__SQL";

        public LogLevel MinimumLevel { get; set; }

        public ILogger CreateLogger(string name)
        {
            //Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
            //var kk = System.Configuration.ConfigurationManager.GetSection("system.xml/xmlReader");

            var l = Logger;

            return l;
        }

        public void AddProvider(ILoggerProvider provider)
        {
            System.Configuration.ConfigurationManager.GetSection("system.xml/xmlReader");
        }

        private static SqlLogger Init()
        {
            var logger = new SqlLogger();

            var evidence = AppDomain.CurrentDomain.Evidence;
            //CallContext.LogicalSetData(ContextName, logger);

            return logger;
        }

        private static SqlLogger Logger
        {
            get
            {
                var logger = new SqlLogger();// CallContext.LogicalGetData(ContextName);

                //var evidence = AppDomain.CurrentDomain.Evidence;
                return logger ?? Init();
            }
        }

        public static CancellationToken CancelQuery()
        {
            Logger._cancellationTokenSource = new CancellationTokenSource();

            return Logger._cancellationTokenSource.Token;
        }

        public static void Reset()
        {
            CallContext.LogicalSetData(ContextName, null);
        }

        //public static void CaptureOutput(ITestOutputHelper testOutputHelper)
        //{
        //    Logger._testOutputHelper = testOutputHelper;
        //}

        //[Serializable]
        private class SqlLogger : ILogger
        {
            private static readonly ILog log = LogManager.GetLogger(typeof(SqlLogger));

            public SqlLogger()
            {
                BasicConfigurator.Configure();
            }

            //public readonly StringBuilder _log = new StringBuilder();
            public void Log(
                LogLevel logLevel,
                int eventId,
                object state,
                Exception exception,
                Func<object, Exception, string> formatter)
            {
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource = null;
                }

                var format = formatter(state, exception)?.Trim();
                log.Info(state);
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public IDisposable BeginScopeImpl(object state)
            {
                return null;
            }

            // ReSharper disable InconsistentNaming

            //public ITestOutputHelper _testOutputHelper;
            public CancellationTokenSource _cancellationTokenSource;

            // ReSharper restore InconsistentNaming
        }
    }
}