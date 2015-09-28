using System.Collections.Generic;

namespace EF7Tests
{
    using Core;
    using DataAccess;
    using DataAccess.Interaces;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Ploeh.AutoFixture;

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
            public void CanRetrieve()
            {
            }

            [TestMethod]
            public void CanRetrieveById()
            {
                var blog = new Blog();
                blog.Name = _fixture.Create<string>();
                using (var uow = UoWFactory.Create())
                {
                    var repository = uow.Create();

                    var createdBlog = repository.Create(blog);

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
                projector
                    .Select(p => p.Name)
                    .Include<Post>(m => m.Posts, p => p.Text);

                using (var uow = UoWFactory.Create())
                {
                    var repository = uow.Create();

                    var retrievedBlogWithPosts = repository.RetrieveById(blog.Id, projector);
                }
            }

            [TestMethod]
            public void CanRetrieveById1()
            {
                #region Arrange

                var blog = new Blog();
                blog.Name = _fixture.Create<string>();
                var post = new Post();
                post.Text = _fixture.Create<string>();

                blog.Posts.AddRange(new List<Post>() { post });

                using (var uow = UoWFactory.Create())
                {
                    var repository = uow.Create();

                    var createdPost = repository.Create(post);

                    uow.Commit();
                }

                #endregion Arrange

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

                #endregion Act

                #region Assert

                var projector = PropertyProjectorFactory<Blog>.Create();
                projector
                    .Select(p => p.Name)
                    .Include<Post>(m => m.Posts, p => p.Date, p => p.Text);

                using (var uow = UoWFactory.Create())
                {
                    var repository = uow.Create();

                    var retrievedBlogWithPosts = repository.RetrieveById(blog.Id, projector);
                }

                #endregion Assert
            }


            [TestMethod]
            public void CanRetrieveByIdBuildingQueryCache()
            {
                #region Arrange

                var blog = new Blog();
                blog.Name = _fixture.Create<string>();
                var post = new Post();
                post.Text = _fixture.Create<string>();

                blog.Posts.AddRange(new List<Post>() { post });

                using (var uow = UoWFactory.Create())
                {
                    var repository = uow.Create();

                    var createdPost = repository.Create(post);

                    uow.Commit();
                }

                #endregion Arrange

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

                #endregion Act

                #region Assert

  

                using (var uow = UoWFactory.Create())
                {                   
                    var repository = uow.Create();

                    IPropertyProjectorBuilder<Blog> builder
                        = repository.CreatePropertyProjectorBuilder<Blog>(blog);

                    var pp = builder
                        .Select(p => p.Name)
                        .IncludeNew<Post>(m => m.Posts, p => p.Date, p => p.Text)
                        .Build();

                    var retrievedBlogWithPosts = repository.RetrieveByIdNew(blog.Id, pp);
                }

                using (var uow = UoWFactory.Create())
                {
                    var repository = uow.Create();

                    IPropertyProjectorBuilder<Blog> builder
                        = repository.CreatePropertyProjectorBuilder<Blog>(blog);

                    var pp = builder
                        .Select(p => p.Name)
                        .IncludeNew<Post>(m => m.Posts, p => p.Date, p => p.Text)
                        .Build();

                    var retrievedBlogWithPosts = repository.RetrieveByIdNew(blog.Id, pp);
                }

                #endregion Assert
            }
        }
    }
}