using DataAccess.Interaces;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Framework.DependencyInjection;

namespace DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private EF7BloggContext context;

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
        }

        public void Dispose()
        {
        }
    }
}