using DataAccess.Interaces;
using EF7;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;

namespace EF7Tests
{
    /// <summary>
    /// Summary description for NonGenericTests
    /// </summary>
    [TestClass]
    public class NonGenericTests : TestBase
    {
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
                    Name = Fixture.Create<string>()
                };

                post = new Post();
                post.Text = Fixture.Create<string>();
                post.Date = Fixture.Create<string>();
                post.Blog = blog;

                blog.Posts.Add(post);

                createdBlog = rep.AddWithRelations(blog);

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