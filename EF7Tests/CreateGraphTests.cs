using Core;
using DataAccess;
using DataAccess.Interaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;
using System.Linq;

namespace EF7Tests
{
    [TestClass]
    public class CreateGraphTests
    {
        private Fixture _fixture;

        public CreateGraphTests()
        {
            _fixture = new Fixture();
        }

        [TestMethod]
        public void CanCreateGraph()
        {
            #region Arrange

            var blog = new Blog();
            blog.Name = _fixture.Create<string>();

            var post = new Post();
            post.Text = _fixture.Create<string>();
            blog.Posts.Add(post);

            #endregion Arrange

            #region Act

            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedBlog = repository.CreateGraph(blog);
                uow.Commit();
            }

            #endregion Act

            #region Assert

            // TODO!

            #endregion Assert
        }

        [TestMethod]
        public void CanCreateGraph2()
        {
            #region Arrange

            Blog createdBlog;
            Post post;

            var blog = new Blog
            {
                Name = _fixture.Create<string>(),
                Description = _fixture.Create<string>()
            };

            post = new Post()
            {
                Text = _fixture.Create<string>(),
                Date = _fixture.Create<string>(),
                Blog = blog,
            };

            var follower = new Follower()
            {
                Name = _fixture.Create<string>(),
                Blog = blog
            };

            blog.Posts.Add(post);
            blog.Followers.Add(follower);

            post = new Post();
            post.Text = _fixture.Create<string>();
            post.Date = _fixture.Create<string>();
            post.Blog = blog;

            blog.Posts.Add(post);

            #endregion Arrange

            #region Act

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();

                createdBlog = rep.CreateGraph(blog);

                uow.Commit();
            }

            #endregion Act

            #region Assert

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();

                var projector = PropertyProjectorFactory<Blog>.Create();
                var projection = projector
                    .Select(m => m.Id, m => m.Name, m => m.Description)
                    .Include<Post>(m => m.Posts, p => p.Id, p => p.Date, p => p.Text)
                    .Include<Follower>(m => m.Followers, p => p.Id, p => p.Name);

                var retrievedBlog = rep.RetrieveById(createdBlog.Id, projection);
            }

            #endregion Assert
        }

        [TestMethod]
        public void CanCreateGraph_Inconclusive()
        {
            Assert.Inconclusive("This implementation of retrieve is not going to be used");
            Post post;
            Blog createdBlog;
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

                post = new Post();
                post.Text = _fixture.Create<string>();
                post.Date = _fixture.Create<string>();
                post.Blog = blog;

                blog.Posts.Add(post);

                createdBlog = rep.CreateGraph(blog);

                uow.Commit();
            }

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();

                var retrievedBlogWithPosts = rep.RetrieveObsolete<Blog, dynamic>(
                      createdBlog.Id, p => new { ff = p.Name, ffff = p.Description, fff = p.Posts.Select(pp => pp.Text) });
            }
        }
    }
}