using EF7;
using Microsoft.Data.Entity;
using System;

namespace DataAccess
{
    public class EF7BloggContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        public EF7BloggContext()
        {
        }

        public EF7BloggContext(IServiceProvider provider)
            : base(provider)
        {
            //var serviceProvider = new ServiceCollection()
            //    .AddEntityFramework()
            //    .AddSqlServer()
            //    .GetService()
            //    .BuildServiceProvider();

            //var f = serviceProvider.GetService<ILoggerFactory>();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            base.OnConfiguring(options);

            //options.UseSqlServer("Server=(localdb)\\EF7;Database=EF7;User Id=Tobias;Password=Tobias;MultipleActiveResultSets=True").MaxBatchSize(20);
            options.UseSqlServer("Server=(localdb)\\ProjectsV12;Database=EF7;Trusted_Connection=true;MultipleActiveResultSets=True").
                MaxBatchSize(20);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<PreRegistration>().Property(p => p.ID).GenerateValueOnAdd();
            //modelBuilder.Entity<PreRegistration>().Key(p => p.ID);
            ////modelBuilder.Entity<Blogg>().Property(p => p.Id).GenerateValueOnAdd();
            //modelBuilder.Model.GetEntityType(typeof(Meeting)).GetProperty("ID").GenerateValueOnAdd = true;
            //modelBuilder.Entity<Meeting>().Key(p => p.ID);
            //modelBuilder.Entity<Meeting>().Property(p => p.ID).GenerateValueOnAdd();
            modelBuilder.Entity<Blog>()
                .Collection(b => b.Posts)
                .InverseReference(b => b.Blog)
                .ForeignKey(k => k.BlogId);
        }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<Blogg>().HasMany<Post>(b => b.Posts).WithOptional(b => b.Blogg).HasForeignKey(k => k.BloggId);
        //}
    }
}