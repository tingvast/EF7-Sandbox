using Core;
using DataAccess;
using Microsoft.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;
using System;
using System.Linq;

namespace EF7Tests
{
    [TestClass]
    public class EF7BloggContextTests : TestBase
    {
  

        [TestMethod]
        public void CanRetrieveAndAllNavigationPropertiesArePopulated()
        {
            var blog = new Blog()
            {
                Author = Fixture.Create<string>(),
                Name = Fixture.Create<string>(),
                Posts = new System.Collections.Generic.List<Post>()
                {
                    new Post() { Date = "20150909", Text = Fixture.Create<string>(), Url = Guid.NewGuid().ToString()},
                    new Post() { Date = "20150909", Text = Fixture.Create<string>(), Url =  Guid.NewGuid().ToString()}
                }
            };

            using (var db = new EF7BloggContext())
            {
                db.ChangeTracker.TrackGraph(blog, c => c.State = EntityState.Added);

                db.SaveChanges();
            };

            using (var db = new EF7BloggContext())
            {
                var retrievedBlog = db
                    .Blogs.Include(b => b.Posts)
                    .Single(b => b.Id == blog.Id);
            };
        }

        [TestMethod]
        public void CanNotCrash()
        {
            //var blog = new Blog()
            //{
            //    Author = Fixture.Create<string>(),
            //    Name = Fixture.Create<string>(),
            //    Posts = new System.Collections.Generic.List<Post>()
            //    {
            //        new Post() { Date = "20150909", Text = Fixture.Create<string>() },
            //        //new Post() { Date = "20150909", Text = Fixture.Create<string>() }
            //    }
            //};

            var post = new Post()
            {
                Text = Fixture.Create<string>(),
                Url = Fixture.Create<string>(),
            };

            using (var db = new EF7BloggContext())
            {
                db.ChangeTracker.TrackGraph(post, c => c.State = EntityState.Added);

                db.SaveChanges();
            };
        }

        [TestMethod]
        public void CanSelectDynamic()
        {
            using (var db = new EF7BloggContext())
            {
                var authorToSerachFor = Fixture.Create<string>();
                var blog = new Blog() { Name = Fixture.Create<string>(), Description = Fixture.Create<string>() };

                db.AddRange(new[] { blog });

                db.SaveChanges();

                var retrievedBlog = db.Set<Blog>()
                    .Include(p => p.Posts)
                    .ToList();
                //.Select(p => new { p.Author })
                //.ToList();
                //.SelectDynamic(new[] { nameof(Blog.Author) });

                foreach (var blog1 in retrievedBlog)
                {
                    Assert.IsNotNull(blog1);
                }
            }
        }
    }
}