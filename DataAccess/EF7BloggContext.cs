﻿using Core;
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
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            base.OnConfiguring(options);

            options.UseSqlServer("Server=(localdb)\\ProjectsV12;Database=EF7;Trusted_Connection=true;MultipleActiveResultSets=True").
                MaxBatchSize(20);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Blog>()
                .Collection(b => b.Posts)
                .InverseReference(b => b.Blog)
                .ForeignKey(k => k.BlogId);

            modelBuilder.Entity<TrackBack>()
                .Reference<Post>(t => t.Post)
                .InverseCollection(p => p.TrackBacks)
                .ForeignKey(t => t.PostUrl)
                .PrincipalKey(p => p.Url);

            modelBuilder.Entity<Post>()
                .Property(p => p.Id)
                .UseSqlServerIdentityColumn();
        }
    }
}