using Core;
using DataAccess.Interaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;

namespace EF7Tests
{
    /// <summary>
    /// Summary description for UpdateTests
    /// </summary>
    [TestClass]
    public class UpdateTests
    {
        private Fixture _fixture;

        public UpdateTests()
        {
            _fixture = new Fixture();
        }

        [TestMethod]
        public void CanUpdateBusinessObject()
        {
            #region Arrange

            Post retrievedPost = null;
            using (var uow = UoWFactory.Create())
            {
                Post post;
                var rep = uow.Create();
                post = new Post();
                post.BlogText = _fixture.Create<string>();
                post.Date = _fixture.Create<string>();

                var persistedPost = rep.Create<Post>(post);

                uow.Commit();

                var projector = rep.CreatePropertyProjectorBuilder(post)
                    .Select(p => p.BlogText, p => p.Date)
                    .Build();

                retrievedPost = rep.RetrieveById(persistedPost.Id, projector);
            }

            #endregion Arrange

            #region Act

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                retrievedPost.BlogText = _fixture.Create<string>();
                retrievedPost.Date = _fixture.Create<string>();

                rep.Update(retrievedPost, p => p.BlogText);

                uow.Commit();
            }

            #endregion Act
        }

        [TestMethod]
        public void CanUpdateSelectedProperties()
        {
            #region Arrange

            var blog = new Blog();
            blog.Name = _fixture.Create<string>();
            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedBlog = repository.UpdateGraph(blog);

                uow.Commit();
            }

            #endregion Arrange

            #region Act

            blog.Name = _fixture.Create<string>();
            blog.Author = _fixture.Create<string>();

            using (var uow1 = UoWFactory.Create())
            {
                var repository = uow1.Create();

                // Only name should be updated
                var updatedBlog = repository.Update(blog, p => p.Name);

                uow1.Commit();
            }

            #endregion Act

            #region Assert

            var uow2 = UoWFactory.Create();
            var r = uow2.Create();

            #endregion Assert
        }
    }
}