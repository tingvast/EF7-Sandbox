using Core;
using DataAccess.Interaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;

namespace EF7Tests
{
    /// <summary>
    /// Summary description for NonGenericTests
    /// </summary>
    [TestClass]
    public class NonGenericTests
    {
        private Fixture _fixture;

        public NonGenericTests()
        {
            _fixture = new Fixture();
        }

        [TestMethod]
        public void CanRetrieveBlogNonGeneric()
        {
            #region Arrange

            Blog createdBlog;
            Post post;
            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                var blog = new Blog
                {
                    Name = _fixture.Create<string>()
                };

                post = new Post();
                post.Text = _fixture.Create<string>();
                post.Date = _fixture.Create<string>();
                post.Blog = blog;

                blog.Posts.Add(post);

                createdBlog = rep.CreateGraph(blog);

                uow.Commit();
            }

            #endregion Arrange

            #region Act

            Blog retrievedBlog = null;
            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                retrievedBlog = rep.RetrieveBlogNonGeneric(createdBlog.Id);
            }

            #endregion Act

            #region Assert

            Assert.IsNotNull(retrievedBlog);

            #endregion Assert
        }
    }
}