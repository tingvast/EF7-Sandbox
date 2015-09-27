using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;
using Core;
using DataAccess.Interaces;

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
                    Author = _fixture.Create<string>()
                };

                post = new Post();
                post.Text = _fixture.Create<string>();
                post.Date = _fixture.Create<string>();
                post.Blog = blog;

                blog.Posts.Add(post);

                createdBlog = rep.CreateGraph(blog);

                uow.Commit();
            }

            #endregion

            #region Act
            Blog retrievedBlog = null;
            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                retrievedBlog = rep.RetrieveBlogNonGeneric(createdBlog.Id);
            }

            #endregion

            #region Assert

            Assert.IsNotNull(retrievedBlog);

            #endregion


        }
    }
}
