using Core;
using DataAccess;
using DataAccess.Interaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;

namespace EF7Tests
{
    /// <summary>
    /// Summary description for Retrieve
    /// </summary>
    [TestClass]
    public class RetrieveTests
    {
        private Fixture _fixture;

        public RetrieveTests()
        {
            _fixture = new Fixture();
        }

        [TestMethod]
        public void CanRetrieveBusinessObject()
        {
            using (var uow = UoWFactory.Create())
            {
                #region Arrange

                var rep = uow.Create();
                var blog = new Blog
                {
                    Name = _fixture.Create<string>()
                };

                var createdBlog = rep.Create<Blog>(blog);

                uow.Commit();

                #endregion Arrange

                #region Act

                var retrievedBlog = rep.Retrieve<Blog>(createdBlog.Id);

                #endregion Act

                #region Assert

                // TODO

                #endregion Assert
            }
        }

        [TestMethod]
        public void CanRetrieveByIdSelectedPropertiesFromBusinessObject()
        {
            using (var uow = UoWFactory.Create())
            {
                #region Arrange

                var rep = uow.Create();
                var blog = new Blog
                {
                    Name = _fixture.Create<string>()
                };

                var createdBlog = rep.Create<Blog>(blog);

                uow.Commit();

                #endregion Arrange

                #region Act

                var projector = PropertyProjectorFactory<Blog>.Create();
                projector.Select(p => p.Name)
                    .Include<Post>(p => p.Text);

                var retrievedBlog = rep.RetrieveById<Blog>(createdBlog.Id, projector);

                #endregion Act

                #region Assert

                // TODO

                #endregion Assert
            }
        }

        [TestMethod]
        public void CanRetrieveBusinessObjectDiconnected()
        {
            Blog createdBlog = null;

            #region Arrange

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                var blog = new Blog
                {
                    Name = _fixture.Create<string>()
                };

                createdBlog = rep.Create<Blog>(blog);

                uow.Commit();
            }

            #endregion Arrange

            #region Act

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                var retrievedBlog = rep.Retrieve<Blog>(createdBlog.Id);
            }

            #endregion Act
        }
    }
}