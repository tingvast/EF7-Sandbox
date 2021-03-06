﻿using Microsoft.Data.Entity.Query;
using Microsoft.Data.Entity.Update;
using Microsoft.Data.Entity.Update.Internal;
using Microsoft.Framework.Logging;
using System;
using System.Linq;

namespace DataAccess.Logging
{
    public class SqlLoggerProvider : ILoggerProvider
    {
        private static readonly string[] _sqlGenerationComponents = new string[]
        {
            typeof(BatchExecutor).FullName,
            typeof(QueryContextFactory).FullName
        };

        public ILogger CreateLogger(string name)
        {
            if (_sqlGenerationComponents.Contains(name))
            {
                return new SqlLogger();
            }

            return NullLogger.Instance;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}