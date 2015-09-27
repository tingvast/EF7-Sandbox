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
    public class CreateTests
    {
        private Fixture _fixture;

        public CreateTests()
        {
            _fixture = new Fixture();
        }

        [TestMethod]
        public void CanCreateBusinessObject()
        {
            #region Arrange

            var blog = new Blog
            {
                Name = _fixture.Create<string>()
            };

            #endregion Arrange

            #region Act

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();

                var persistedBlog = rep.Create<Blog>(blog);

                uow.Commit();
            }

            #endregion Act

            #region Assert

            // TODO.

            #endregion Assert
        }

        [TestMethod]
        public void CanCreateBusinessObjectWithoutRoundtrips()
        {
            #region Arrange
            var blog1 = new Blog() { Name = _fixture.Create<string>() };
            var blog2 = new Blog() { Name = _fixture.Create<string>() };
            var blog3 = new Blog() { Name = _fixture.Create<string>() };
            #endregion

            #region Act
            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();

                // This will generate one batch sql command to the database (instead of as in previous versions on EF one for each object.)
                var persistedBlog = rep.CreateMany<Blog>(new[] { blog1, blog2, blog3 });

                uow.Commit();
            }
            #endregion

            #region Assert
            // TODO
            #endregion

        }

        [TestMethod]
        public void CanCreateAnotherBusinessObject()
        {
            #region Arrange

            Post post;

            post = new Post();
            post.Text = _fixture.Create<string>();
            post.Date = _fixture.Create<string>();

            #endregion Arrange

            #region Act

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                var persitedPost = rep.Create<Post>(post);

                uow.Commit();
            }

            #endregion Act

            #region Assert

            // TODO.

            #endregion Assert
        }

        [TestMethod]
        [ExpectedException(typeof(Microsoft.Data.Entity.DbUpdateException))]
        public void CanNotCreateTheSameBusinessObjectExpectedException()
        {
            #region Arrange

            Post post;

            post = new Post();
            post.Text = _fixture.Create<string>();
            post.Date = _fixture.Create<string>();

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                var persitedPost = rep.Create<Post>(post);

                uow.Commit();

                #endregion Arrange

                #region Act

                persitedPost = rep.Create<Post>(post);

                uow.Commit();

                #endregion Act
            }

            #region Assert

            // TODO.

            #endregion Assert
        }

        [TestMethod]
        [ExpectedException(typeof(Microsoft.Data.Entity.DbUpdateException))]
        public void CanNotCreateTheSameBusinessObjectUsingDisconnectedScenarioExpectedException()
        {
            #region Arrange

            Post post;

            post = new Post();
            post.Text = _fixture.Create<string>();
            post.Date = _fixture.Create<string>();

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                var persitedPreRegistration = rep.Create<Post>(post);

                uow.Commit();
            }

            #endregion Arrange

            #region Act

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                var persitedPost = rep.Create<Post>(post);

                uow.Commit();
            }

            #endregion Act

            #region Assert

            // TODO.

            #endregion Assert
        }
    }
}