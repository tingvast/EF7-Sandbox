using Core;
using DataAccess.Interaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;

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
        public void CanAddTheSameBusinessObjectExpectedException()
        {
            #region Arrange

            Post post;

            post = new Post();
            post.Text = Fixture.Create<string>();
            post.Date = Fixture.Create<string>();

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
        public void CanAddTheSameBusinessObjectUsingDisconnectedScenarioExpectedException()
        {
            #region Arrange

            Post post;

            post = new Post();
            post.Text = Fixture.Create<string>();
            post.Date = Fixture.Create<string>();

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