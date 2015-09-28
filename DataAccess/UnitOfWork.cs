using DataAccess.Interaces;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Framework.DependencyInjection;
using System.Linq;

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
            foreach (var e in context.ChangeTracker.Entries())
            {
                var state = e.State;
                var type = e.Entity.GetType();
            }

            context.SaveChanges();

            var allEntities = context.ChangeTracker.Entries().ToList();

            //for (int i = 0; i < allEntities.Count; i++)
            //{
            //    allEntities[i].State = Microsoft.Data.Entity.EntityState.Detached;
            //}
        }

        public void Dispose()
        {
        }
    }
}