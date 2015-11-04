using Core;
using DataAccess;
using Microsoft.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;
using System.Linq;

namespace EF7Tests
{
    [TestClass]
    public class ShowAndTell : TestBase
    {
        /*
        1. Gå igenom domänmodellen och dess syfte, data och beteende
        2.
        */

        [TestMethod]
        public void TestEf7SqlGenerationImprovements()
        {
            using (var db = new EF7BloggContext(ServiceProvider))
            {
                //EnsureCleanDatabase(db);

                // EF6 three roundtrips to db
                db.Blogs.Add(new Blog() { Name = "ADO.NET Blog" });
                db.Blogs.Add(new Blog() { Name = "Visual Studio" });
                db.Blogs.Add(new Blog() { Name = ".NET Blog" });

                db.SaveChanges();
            }
        }

        [TestMethod]
        public void CanRetriveUsingTableValuedFunction()
        {
            /*
			USE [EF7]
			GO

			SET ANSI_NULLS ON
			GO

			SET QUOTED_IDENTIFIER ON
			GO

			DROP FUNCTION[dbo].[SearchBlogs];

			GO
			CREATE FUNCTION[dbo].[SearchBlogs]
			(
				@param2 nvarchar(200)
			)
			RETURNS @returntable TABLE
			(
				Id int,
				Author nvarchar(200),
				Name nvarchar(200),
                Description nvarchar(200)
			)
			AS
			BEGIN
				INSERT @returntable
				SELECT
                    blogs.Id, blogs.Name, blogs.Author, blogs.Description
                from dbo.Blog blogs
                where
                    blogs.Name like @param2
				RETURN
			END
			*/

            using (var db = new EF7BloggContext(ServiceProvider))
            {
                //EnsureCleanDatabase(db);

                db.Blogs.Add(new Blog() { Name = "ADO.NET Blog" });
                db.Blogs.Add(new Blog() { Name = "Visual Studio" });
                db.Blogs.Add(new Blog() { Name = ".NET Blog" });

                db.SaveChanges();

                /*
                1. Parameterized, not open to sql injection
                2. Strongly typed (renaming properties)
                3. The order by is happening in the database, not in memory.
                */
                var nameToSerachFor = "blog";
                var seachedBlogs = db.Set<Blog>()
                    .FromSql("select * from dbo.SearchBlogs (@p0)", nameToSerachFor);
                //.OrderBy(b => b.Name); // <-- Strongly typed

                foreach (var blog in seachedBlogs)
                {
                    var found = blog.Name;
                }
            }
        }

        [TestMethod]
        public void CanPerformSomeInMermoryAndSomeInDatabase()
        {
            using (var db = new EF7BloggContext(ServiceProvider))
            {
                //EnsureCleanDatabase(db);

                db.Blogs.Add(new Blog() { Name = "ADO.NET Blog" });
                db.Blogs.Add(new Blog() { Name = "Visual Studio" });
                db.Blogs.Add(new Blog() { Name = ".NET Blog" });

                db.SaveChanges();

                var seachedBlogs = db.Set<Blog>()
                    .OrderBy(b => GetSortName(b.Name))
                    .Where(b => b.Name.Contains(("blog"))); // Some parts of the query in db (where) and some inmemory (GetSortName)

                foreach (var blog in seachedBlogs)
                {
                    var found = blog.Name;
                }
            }
        }

        [TestMethod]
        public void CanIncludeNavigationProperties()
        {
            using (var db = new EF7BloggContext(ServiceProvider))
            {
                //EnsureCleanDatabase(db);

                db.Blogs.Add(new Blog() { Name = "ADO.NET Blog" });
                db.Blogs.Add(new Blog() { Name = "Visual Studio" });
                db.Blogs.Add(new Blog() { Name = ".NET Blog" });

                db.SaveChanges();

                var seachedBlogs = db.Set<Blog>()
                    .Include((b => b.Posts))        // Eager loading, in ef6 10 blog with 10 posts each
                    .Where(b => b.Name.Contains("blog"));

                foreach (var blog in seachedBlogs)
                {
                    var found = blog.Name;
                }
            }
        }

        [TestMethod]
        public void CanIncludeThenIncludeNavigationProperties()
        {
            using (var db = new EF7BloggContext(ServiceProvider))
            {
                //EnsureCleanDatabase(db);

                db.Blogs.Add(new Blog() { Name = "ADO.NET Blog" });
                db.Blogs.Add(new Blog() { Name = "Visual Studio" });
                db.Blogs.Add(new Blog() { Name = ".NET Blog" });

                db.SaveChanges();

                var seachedBlogs = db.Set<Blog>()
                    .Include((b => b.Posts))        // Eager loading, in ef6 10 blog with 10 posts each
                    .Where(b => b.Name.Contains("blog"));

                foreach (var blog in seachedBlogs)
                {
                    var found = blog.Name;
                }
            }
        }

        [TestMethod]
        public void AddSingleObject()
        {
            /*
            Purpose: To test adding one single object to the database, without navigation properties
            */
            using (var con = new EF7BloggContext(ServiceProvider))
            {
                var blog = Fixture.Build<Blog>()
                    .OmitAutoProperties()
                    .With(p => p.Author, Fixture.Create<string>())
                    .With(p => p.Name, Fixture.Create<string>())
                    .With(p => p.Description, Fixture.Create<string>())
                    .Create();

                con.Blogs.Add(blog);

                con.SaveChanges();
            }
        }

        [TestMethod]
        public void AddComplexObject()
        {
            /*
            Purpose: To test adding object with one relation (navigation property Posts)
            */
            using (var con = new EF7BloggContext(ServiceProvider))
            {
                var blog = Fixture.Build<Blog>()
                    .OmitAutoProperties()
                    .With(p => p.Author, Fixture.Create<string>())
                    .With(p => p.Name, Fixture.Create<string>())
                    .With(p => p.Description, Fixture.Create<string>())
                    .With(p => p.Posts, Fixture.CreateMany<Post>().ToList())
                    .Create();

                con.ChangeTracker.TrackGraph(blog, e => e.Entry.State = EntityState.Added);

                con.SaveChanges();
            }
        }

        [TestMethod]
        public void AddEvenMoreComplexObject()
        {
            /*
            Purpose: To test adding object with one relation (navigation property Posts)
            */
            using (var con = new EF7BloggContext(ServiceProvider))
            {
                var blog = Fixture.Build<Blog>()
                    .OmitAutoProperties()
                    .With(p => p.Author, Fixture.Create<string>())
                    .With(p => p.Name, Fixture.Create<string>())
                    .With(p => p.Description, Fixture.Create<string>())
                    .With(p => p.Posts, Fixture.CreateMany<Post>().ToList())
                    .With(p => p.Followers, Fixture.CreateMany<Follower>().ToList())
                    .Create();

                con.ChangeTracker.TrackGraph(blog, e => e.Entry.State = EntityState.Added);

                con.SaveChanges();
            }
        }

        [TestMethod]
        public void AddDisconnected()
        {
            /*
            Purpose: To test adding a simple object with no relations, in the first session using one context
                     Later add a relation to object using another session (context)
            */

            Blog blog = null;
            using (var con = new EF7BloggContext(ServiceProvider))
            {
                blog = Fixture.Build<Blog>()
                    .OmitAutoProperties()
                    .With(p => p.Author, Fixture.Create<string>())
                    .With(p => p.Name, Fixture.Create<string>())
                    .With(p => p.Description, Fixture.Create<string>())
                    .Create();

                con.ChangeTracker.TrackGraph(blog, e => e.Entry.State = EntityState.Added);

                con.SaveChanges();
            }

            using (var con = new EF7BloggContext(ServiceProvider))
            {
                var post = Fixture.Build<Post>()
                    .OmitAutoProperties()
                    .With(p => p.Text, Fixture.Create<string>())
                    .With(p => p.Date, Fixture.Create<string>())
                    .With(p => p.Url, Fixture.Create<string>())
                    .With(p => p.Blog, blog)
                    .Create();

                con.ChangeTracker.TrackGraph(post, e =>
                {
                    if (e.Entry.IsKeySet)
                    {
                        e.Entry.State = EntityState.Modified;
                    }
                    else
                    {
                        e.Entry.State = EntityState.Added;
                    }
                });

                con.SaveChanges();
            }
        }

        #region Private

        private void EnsureCleanDatabase(EF7BloggContext con)
        {
            con.Database.EnsureDeleted();
            con.Database.EnsureCreated();
        }

        private string GetSortName(string name)
        {
            if (name.StartsWith((".")))
            {
                return "dot" + name.Substring((1));
            }

            return name;
        }

        #endregion Private
    }
}