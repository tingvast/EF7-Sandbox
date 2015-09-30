using DataAccess.Interaces;
using EF7;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;

namespace EF7Tests
{
    /// <summary>
    /// Summary description for UpdateTests
    /// </summary>
    [TestClass]
    public class UpdateTests : TestBase
    {
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
                post.Text = Fixture.Create<string>();
                post.Date = Fixture.Create<string>();

                var persistedPost = rep.Add<Post>(post);

                uow.Commit();

                var projector = rep.PropertySelectBuilder(post)
                    .Select(p => p.Text, p => p.Date)
                    .Build();

                retrievedPost = rep.RetrieveById(persistedPost.Id, projector);
            }

            #endregion Arrange

            #region Act

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                retrievedPost.Text = Fixture.Create<string>();
                retrievedPost.Date = Fixture.Create<string>();

                rep.Update(retrievedPost, p => p.Text);

                uow.Commit();
            }

            #endregion Act
        }

        [TestMethod]
        public void CanUpdateSelectedProperties()
        {
            #region Arrange

            var blog = new Blog();
            blog.Name = Fixture.Create<string>();
            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedBlog = repository.UpdateGraph(blog);

                uow.Commit();
            }

            #endregion Arrange

            #region Act

            blog.Name = Fixture.Create<string>();
            blog.Author = Fixture.Create<string>();

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