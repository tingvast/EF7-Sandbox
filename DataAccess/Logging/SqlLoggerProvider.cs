using Microsoft.Data.Entity.Query;
using Microsoft.Data.Entity.Update;
using Microsoft.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Logging
{
    internal class SqlLoggerProvider : ILoggerProvider
    {
        private static readonly string[] _sqlGenerationComponents = new string[]
        {
            typeof(BatchExecutor).FullName,
            typeof(QueryContextFactory).FullName
        };

        public ILogger CreateLogger(string name)
        {
           if(_sqlGenerationComponents.Contains(name))
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
