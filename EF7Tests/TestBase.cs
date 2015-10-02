using DataAccess.Logging;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Ploeh.AutoFixture;
using System;

namespace EF7Tests
{
    public class TestBase
    {
        protected Fixture Fixture;

        protected IServiceProvider ServiceProvider;

        public TestBase()
        {
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            Fixture = new Fixture();

            Fixture.Register(() => TestDataBuilders.BuildAnyBlogWithRelations(Fixture));
            Fixture.Register(() => TestDataBuilders.BuildAnyFollower(Fixture));
            Fixture.Register(() => TestDataBuilders.BuildAnyPost(Fixture));

            ServiceProvider = new ServiceCollection()
               .AddEntityFramework()
               .AddSqlServer()
               .GetService()
               .BuildServiceProvider();

            ServiceProvider.GetService<ILoggerFactory>().AddProvider(new SqlLoggerProvider());
        }
    }
}