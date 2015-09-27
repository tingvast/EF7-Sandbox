//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using log4net;
//using Xunit.Abstractions;

//namespace DataAccess.Logging
//{
//    [Serializable]
//    class ConsoleLogger : ITestOutputHelper
//    {
//        private static readonly ILog log = LogManager.GetLogger(typeof(ConsoleLogger));
//        public void WriteLine(string message)
//        {
//            log.Debug(message);
//        }

//        public void WriteLine(string format, params object[] args)
//        {
//            throw new NotImplementedException();
//        }

//        public IDisposable Indent()
//        {
//            throw new NotImplementedException();
//        }
//    }
//}