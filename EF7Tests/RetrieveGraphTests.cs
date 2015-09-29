using System.Collections.Generic;

namespace EF7Tests
{
    using Core;
    using DataAccess.Interaces;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Ploeh.AutoFixture;
    using System.Diagnostics;

    namespace EF7Tests
    {
        [TestClass]
        public class RetrieveGraphTests
        {
            private Fixture _fixture;

            public RetrieveGraphTests()
            {
                _fixture = new Fixture();

                _fixture.Register(() => TestDataBuilders.BuildAnyBlog(_fixture));
                _fixture.Register(() => TestDataBuilders.BuildAnyFollower(_fixture));
            }

            [TestMethod]
            public void CanRetrieve()
            {
                using (var uow = UoWFactory.Create())
                {
                    var repository = uow.Create();

                    var blog = _fixture.Create<Blog>();

                    repository.CreateGraph(blog);

                    uow.Commit();

                    blog.Name = "New name";
                    blog.Description = "New Description";
                    foreach (var p in blog.Posts)
                    {
                        p.Text = "New Text";
                    }

                    var pp1 = repository.CreateUpdatePropertyBuilder<Blog>(blog)
                       .PropertiesToUpdate(m => m.Name, m => m.Description)
                       .IncludeNavigationProperyUpdate<Post>(
                            blog.Posts[0].Id,
                            p => p.Posts, p => p.Text)
                        .IncludeNavigationProperyUpdate<Post>(
                            blog.Posts[1].Id,
                            p => p.Posts, p => p.Text)
                       .Build();

                    repository.UpdateGraph(blog, pp1);

                    uow.Commit();

                    var pp = repository.CreatePropertyProjectorBuilder(blog)
                        .Select(p => p.Description, p => p.Name)
                        .Build();

                    var retrieved = repository.RetrieveById(blog.Id, pp);

                    var newFollower = _fixture.Create<Follower>();
                    newFollower.Blog = retrieved;
                    retrieved.Followers.Add(newFollower);

                    repository.Create<Follower>(newFollower);

                    uow.Commit();

                    newFollower.Name = "NewName";

                    var pp2 = repository.CreateUpdatePropertyBuilder<Blog>(blog)
                       .IncludeNavigationProperyUpdate<Follower>(
                            newFollower.Id,
                            p => p.Followers, p => p.Name)
                       .Build();

                    repository.UpdateGraph<Blog>(retrieved, pp2);

                    uow.Commit();
                }
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

                using (var uow = UoWFactory.Create())
                {
                    var repository = uow.Create();

                    var pp = repository.CreatePropertyProjectorBuilder(blog)
                        .Select(m => m.Name)
                        .Include<Post>(p => p.Posts, p => p.Text)
                        .Build();

                    var retrievedBlogWithPosts = repository.RetrieveById(blog.Id, pp);
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

                using (var uow = UoWFactory.Create())
                {
                    var repository = uow.Create();

                    var projector = repository.CreatePropertyProjectorBuilder(blog)
                        .Select(p => p.Name)
                        .Include<Post>(m => m.Posts, p => p.Date, p => p.Text)
                        .Build();

                    var retrievedBlogWithPosts = repository.RetrieveById(blog.Id, projector);
                }

                #endregion Assert
            }

            [TestMethod]
            public void CanRetrieveByIdUsingTheBuildingQueryCache()
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

                    var createdPost = repository.Create(blog);

                    uow.Commit();
                }

                #endregion Arrange

                #region Act

                long first;
                long second;
                using (var uow = UoWFactory.Create())
                {
                    var repository = uow.Create();

                    Stopwatch stopwatch = Stopwatch.StartNew();
                    IPropertyProjectorBuilder<Blog> builder
                        = repository.CreatePropertyProjectorBuilder<Blog>(blog);

                    var pp = builder
                        .Select(p => p.Name)
                        .Include<Post>(m => m.Posts, p => p.Date, p => p.Text)
                        .Build();

                    stopwatch.Stop();
                    first = stopwatch.ElapsedMilliseconds;

                    var retrievedBlogWithPosts = repository.RetrieveById(blog.Id, pp);
                }

                using (var uow = UoWFactory.Create())
                {
                    var repository = uow.Create();

                    Stopwatch stopwatch = Stopwatch.StartNew();
                    IPropertyProjectorBuilder<Blog> builder = repository
                        .CreatePropertyProjectorBuilder<Blog>(blog);

                    var pp = builder
                        .Select(p => p.Name)
                        .Include<Post>(m => m.Posts, p => p.Date, p => p.Text)
                        .Build();

                    second = stopwatch.ElapsedMilliseconds;

                    var retrievedBlogWithPosts = repository.RetrieveById(blog.Id, pp);
                }

                #endregion Act

                #region Assert

                // Assert the latter is faster!

                #endregion Assert
            }
        }
    }
}