using Core;
using DataAccess.Interaces;
using EF7;
using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyNameSpace;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;

namespace DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        EF7BloggContext context;
        public UnitOfWork()
        {
            


            //var loggingFactory = new TestSqlLoggerFactory();
            var serviceProvider = new ServiceCollection()
                   .AddEntityFramework()
                   .AddSqlServer()
                   .GetService()
                   //.AddInstance<ILoggerFactory>(loggingFactory)
                   .BuildServiceProvider();

            this.context = new EF7BloggContext(serviceProvider);
            
            this.context.ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public IRepository Create()
        {
            

            return new Repository(context);
        }

        public void Commit()
        {
            context.SaveChanges();
            //context.Dispose();
        }

        public void Dispose()
        {
         
        }
    }
}
