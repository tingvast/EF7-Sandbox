using Core;
using DataAccess;
using Microsoft.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;
using System.Linq;

namespace EF7Tests
{
    [TestClass]
    public class EF7BloggContextTests
    {
        private Fixture _fixture;

        public EF7BloggContextTests()
        {
            _fixture = new Fixture();
        }

        [TestMethod]
        public void CanRetriveUsingTableValuesFunction()
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
				Location nvarchar(200)
			)
			AS
			BEGIN
				INSERT @returntable
				SELECT blogs.Id, blogs.Author, blogs.Location from dbo.Blog blogs where blogs.Author like @param2
				RETURN
			END
			*/

            using (var db = new EF7BloggContext())
            {
                var nameToSerachFor = _fixture.Create<string>();
                var blog1 = new Blog() { Name = _fixture.Create<string>() };
                var blog2 = new Blog() { Name = nameToSerachFor };
                var blog3 = new Blog() { Name = _fixture.Create<string>() };

                db.AddRange(new[] { blog1, blog2, blog3 });

                db.SaveChanges();

                /*
                1. Parameterized, not open to sql injection
                2. Strongly typed (renaming properties)
                3. The order by is happening in the database, not in memory.
                */
                var seachedBlogs = db.Set<Blog>()
                    .FromSql("select * from dbo.SearchBlogs (@p0)", nameToSerachFor)
                    .OrderBy(b => b.Name); // <-- We know we are going to get back a blog

                foreach (var blog in seachedBlogs)
                {
                    var theOne = blog.Name;
                }
            }
        }

        [TestMethod]
        public void CanRetrieveAndAllNavigationPropertiesArePopulated()
        {
            var blog = new Blog()
            {
                Author = _fixture.Create<string>(),
                Name = _fixture.Create<string>(),
                Posts = new System.Collections.Generic.List<Post>()
                {
                    new Post() { Date = "20150909", BlogText = _fixture.Create<string>() },
                    new Post() { Date = "20150909", BlogText = _fixture.Create<string>() }
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
        public void CanSelectDynamic()
        {
            using (var db = new EF7BloggContext())
            {
                var authorToSerachFor = _fixture.Create<string>();
                var blog = new Blog() { Name = _fixture.Create<string>(), Description = _fixture.Create<string>() };

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