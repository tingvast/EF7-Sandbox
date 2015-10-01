using Core;
using DataAccess.Interaces;
using Microsoft.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;
using System;

namespace EF7Tests
{
    /// <summary>
    /// Summary description for CreateTests
    /// </summary>
    [TestClass]
    public class AddTests : TestBase
    {
        [TestMethod]
        public void CanAddBusinessObject()
        {
            #region Arrange

            var blog = new Blog
            {
                Name = Fixture.Create<string>()
            };

            #endregion Arrange

            #region Act

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();

                var persistedBlog = rep.Add<Blog>(blog);

                uow.Commit();
            }

            #endregion Act

            #region Assert

            // TODO.

            #endregion Assert
        }

        [TestMethod]
        public void CanDoTest()
        {
            //Assert.Inconclusive("This test fails due to invalid test key generation. Running thsi test together with all test, will make it fail due to same key wih different type in cache already");
            // This test fails due to invalid test key generation. Running thsi test together with all test, will make it fail due to same key wih different type in cache already
            Blog blog = Fixture.Create<Blog>();
            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();

                rep.AddWithRelations(blog);

                uow.Commit();
            }

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();

                var retrievedBlog = rep.RetrieveById(blog.Id,
                    rep.PropertySelectBuilder(blog)
                    .Select(p => p.Name)
                    .Build());
            }

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();

                var retrievedBlog = rep.RetrieveById(
                    blog.Id,
                    rep.PropertySelectBuilder(blog)
                    .Select(p => p.Name)
                    .Include<Post>(p => p.Posts, p => p.Text)
                    .Include<Follower>(p => p.Followers, prop => prop.Name)
                    .Build());
            }
        }

        [TestMethod]
        public void CanAddManyBusinessObjectWithoutRoundtrips()
        {
            #region Arrange

            var blog1 = new Blog() { Name = Fixture.Create<string>() };
            var blog2 = new Blog() { Name = Fixture.Create<string>() };
            var blog3 = new Blog() { Name = Fixture.Create<string>() };

            #endregion Arrange

            #region Act

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();

                // This will generate one batch sql command to the database.
                var persistedBlog = rep.Add<Blog>(new[] { blog1, blog2, blog3 });

                uow.Commit();
            }

            #endregion Act

            #region Assert

            // TODO

            #endregion Assert
        }

        [TestMethod]
        public void CanAddAnotherBusinessObject()
        {
            #region Arrange

            Post post;

            post = new Post();
            post.Text = Fixture.Create<string>();
            post.Date = Fixture.Create<string>();
            post.Url = Guid.NewGuid().ToString();

            #endregion Arrange

            #region Act

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                var persitedPost = rep.Add<Post>(post);

                uow.Commit();
            }

            #endregion Act

            #region Assert

            // TODO.

            #endregion Assert
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]
        public void CanAddTheSameBusinessObjectExpectedException()
        {
            /*
            This test should fail since the alternate key Url value is the same between the two insterts.
            */

            #region Arrange

            Post post;

            post = new Post();
            post.Text = Fixture.Create<string>();
            post.Date = Fixture.Create<string>();
            post.Url = Guid.NewGuid().ToString();

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                var persitedPost = rep.Add<Post>(post);

                uow.Commit();

                #endregion Arrange

                #region Act

                persitedPost = rep.Add<Post>(post);

                uow.Commit();

                #endregion Act
            }

            #region Assert

            // TODO.

            #endregion Assert
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]
        public void CanAddTheSameBusinessObjectUsingDisconnectedScenarioExpectedException()
        {
            #region Arrange

            Post post;

            post = new Post();
            post.Text = Fixture.Create<string>();
            post.Date = Fixture.Create<string>();
            post.Url = Guid.NewGuid().ToString();

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                var persitedPreRegistration = rep.Add<Post>(post);

                uow.Commit();
            }

            #endregion Arrange

            #region Act

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                var persitedPost = rep.Add<Post>(post);

                uow.Commit();
            }

            #endregion Act

            #region Assert

            // TODO.

            #endregion Assert
        }
    }
}