using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Metadata;
using Core;

namespace DataAccess
{
    public class EF7BloggContext : DbContext
    {
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<PreRegistration> Preregistrations {get; set;}

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
            modelBuilder.Entity<Meeting>().
                Collection(b => b.PreRegistrations).
                InverseReference(b => b.Meeting).
                ForeignKey(k => k.MeetingId);
           
        }
        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<Blogg>().HasMany<Post>(b => b.Posts).WithOptional(b => b.Blogg).HasForeignKey(k => k.BloggId);
        //}
    }
}
