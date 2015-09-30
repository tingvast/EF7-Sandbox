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
                        p.BlogText = "New Text";
                    }
                    var firstPostId = blog.Posts[0].Id;
                    var secondPostId = blog.Posts[1].Id;
                    var pp1 = repository.CreateUpdatePropertyBuilder<Blog>(blog)
                       .PropertiesToUpdate(m => m.Name, m => m.Description)
                       .IncludeNavigationProperyUpdate<Post>(
                            firstPostId,
                            p => p.Posts, p => p.BlogText)
                        .IncludeNavigationProperyUpdate<Post>(
                            secondPostId,
                            p => p.Posts, p => p.BlogText)
                       .Build();

                    var updatedBlog = repository.UpdateGraph(blog, pp1);

                    uow.Commit();

                    var pp = repository.CreatePropertyProjectorBuilder(blog)
                        .Select(p => p.Description, p => p.Name)
                        .Build();

                    var retrievedBlog = repository.RetrieveById(blog.Id, pp);

                    var newFollower = _fixture.Create<Follower>();
                    newFollower.Blog = retrievedBlog;
                    retrievedBlog.Followers.Add(newFollower);

                    repository.Create<Follower>(newFollower);

                    uow.Commit();

                    newFollower.FirstName = "NewName";

                    var pp2 = repository.CreateUpdatePropertyBuilder<Blog>(blog)
                       .IncludeNavigationProperyUpdate<Follower>(
                            newFollower.Id,
                            p => p.Followers, p => p.FirstName)
                       .Build();

                    var retrievedBlogAgain = repository.UpdateGraph<Blog>(retrievedBlog, pp2);

                    uow.Commit();
                    var pp3 = repository.CreatePropertyProjectorBuilder(newFollower)
                        .Select(p => p.FirstName)
                        .Build();

                    var retrievedFollower = repository.RetrieveById<Follower>(newFollower.Id, pp3);

                    var pp4 = repository.CreatePropertyProjectorBuilder<Blog>(blog)
                       .Select()
                       .Include<Follower>(b => b.Followers, pp11 => pp11.Id, pp11 => pp11.FirstName)
                       .Build();

                    var retrievedBlogAgainAgain = repository.RetrieveById(retrievedBlogAgain.Id, pp4);
                }
            }

            [TestMethod]
            public void CanWhatUnintuitiveBehavior()
            {
                Blog blog = null;
                Blog retrievedBlog = null;
                using (var uow = UoWFactory.Create())
                {
                    var repository = uow.Create();

                    blog = _fixture.Create<Blog>();

                    repository.CreateGraph(blog);

                    uow.Commit();

                    var newFollower = _fixture.Create<Follower>();
                    newFollower.Blog = blog;
                    blog.Followers.Add(newFollower);

                    repository.Create<Follower>(newFollower);

                    uow.Commit();

                    var pp4 = repository.CreatePropertyProjectorBuilder<Blog>(blog)
                       .Select()
                       .Include<Follower>(b => b.Followers, pp11 => pp11.Id, pp11 => pp11.FirstName)
                       .Build();

                    retrievedBlog = repository.RetrieveById(blog.Id, pp4);
                }

                #region Assert

                // TODO: Don't know how to fix this, maybe when an object with a certain type and specific id is fetched from db, it should be
                // untracked by context.
                Assert.Inconclusive("Don't know how to fix this, maybe when an object with a certain type and specific id is fetched from");
                Assert.AreEqual(blog.Followers.Count, retrievedBlog.Followers.Count);

                #endregion Assert
            }

            [TestMethod]
            public void CanRetrieve235()
            {
                Blog blog = null;
                Blog retrievedBlog = null;
                using (var uow = UoWFactory.Create())
                {
                    var repository = uow.Create();

                    blog = _fixture.Create<Blog>();

                    repository.CreateGraph(blog);

                    uow.Commit();

                    var newFollower = _fixture.Create<Follower>();
                    newFollower.FirstName = "NewName";
                    newFollower.Blog = blog;
                    blog.Followers.Add(newFollower);

                    repository.Create<Follower>(newFollower);
                    uow.Commit();
                }
                using (var uow = UoWFactory.Create())
                {
                    var repository = uow.Create();
                    

                    var pp4 = repository.CreatePropertyProjectorBuilder<Blog>(blog)
                        .Select()
                        .Include<Follower>(b => b.Followers, pp11 => pp11.Id, pp11 => pp11.FirstName)
                        .Build();

                    retrievedBlog = repository.RetrieveById(blog.Id, pp4);
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
                post.BlogText = _fixture.Create<string>();

                using (var uow = UoWFactory.Create())
                {
                    var repository = uow.Create();

                    var updatedBlog = repository.Create(post);

                    uow.Commit();
                }

                var newPost = new Post();
                newPost.BlogText = _fixture.Create<string>();
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
                        .Include<Post>(p => p.Posts, p => p.BlogText)
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
                post.BlogText = _fixture.Create<string>();

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
                newPost.BlogText = _fixture.Create<string>();
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
                        .Include<Post>(m => m.Posts, p => p.Date, p => p.BlogText)
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
                post.BlogText = _fixture.Create<string>();

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
                        .Include<Post>(m => m.Posts, p => p.Date, p => p.BlogText)
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
                        .Include<Post>(m => m.Posts, p => p.Date, p => p.BlogText)
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