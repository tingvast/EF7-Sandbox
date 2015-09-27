using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EF7Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Ploeh.AutoFixture;
    using Core;
    using DataAccess.Interaces;
    using DataAccess;

    namespace EF7Tests
    {



        [TestClass]
        public class RetrieveGraphTests
        {

            private Fixture _fixture;


            public RetrieveGraphTests()
            {
                _fixture = new Fixture();
            }



            [TestMethod]
            public void CanCreate8()
            {
                var blog = new Blog();
                blog.Author = _fixture.Create<string>();
                using (var uow = UoWFactory.Create())
                {
                    var repository = uow.Create();

                    var updatedBlog = repository.Create(blog);

                    uow.Commit();
                }

                var post = new Post();
                post.Text = _fixture.Create<string>();

                using (var uow = UoWFactory.Create())
                {
                    var repository = uow.Create();

                    var updatedBlog = repository.Create(post);

                    uow.Commit();
                }

                var newPost = new Post();
                newPost.Text = _fixture.Create<string>();
                blog.Posts.AddRange(new List<Post>() { newPost, post });

                using (var uow = UoWFactory.Create())
                {
                    var repository = uow.Create();

                    var updatedBlog = repository.UpdateGraph(blog);

                    uow.Commit();
                }

                var projector = PropertyProjectorFactory<Blog>.Create();
                projector.Select(p => p.Author)
                    .Include<Post>(p => p.Text);

                using (var uow = UoWFactory.Create())
                {
                    var repository = uow.Create();

                    var retrievedBlogWithPosts = repository.RetrieveById(blog.Id, projector);

                }

            }


            [TestMethod]
            public void CanUpdate34()
            {
                #region Arrange
                var blog = new Blog();
                blog.Author = _fixture.Create<string>();
                var post = new Post();
                post.Text = _fixture.Create<string>();

                blog.Posts.AddRange(new List<Post>() { post });

                using (var uow = UoWFactory.Create())
                {
                    var repository = uow.Create();

                    var updatedBlog = repository.Create(post);

                    uow.Commit();
                }

                #endregion

                #region Act
                var newPost = new Post();
                newPost.Text = _fixture.Create<string>();
                blog.Posts.AddRange(new List<Post>() { newPost, post });

                using (var uow = UoWFactory.Create())
                {
                    var repository = uow.Create();

                    var updatedBlog = repository.UpdateGraph(blog);

                    uow.Commit();
                }

                #endregion

                #region Assert
                var projector = PropertyProjectorFactory<Blog>.Create();
                projector.Select(p => p.Author)
                    .Include<Post>(p => p.Text);

                using (var uow = UoWFactory.Create())
                {
                    var repository = uow.Create();

                    var retrievedBlogWithPosts = repository.RetrieveById(blog.Id, projector);

                }
                #endregion
            }



        }
    }
}
