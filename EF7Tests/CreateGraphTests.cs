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
            Assert.Inconclusive("This implementation of retrieve is not going to be used");
            Post post;
            Blog createdBlog;
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

                var retrievedBlogWithPosts = rep.Retrieve<Blog, dynamic>(
                      createdBlog.Id, p => new { ff = p.Author, ffff = p.Location, fff = p.Posts.Select(pp => pp.Text) });
            }
        }

        [TestMethod]
        public void CanCreateGraph2()
        {
            Post post;
            Blog createdBlog;
            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                var blog = new Blog
                {
                    Author = _fixture.Create<string>(),
                    Location = _fixture.Create<string>()
                };

                post = new Post();
                post.Text = _fixture.Create<string>();
                post.Date = _fixture.Create<string>();
                post.Blog = blog;

                var prereg2 = new Follower();
                prereg2.Name = _fixture.Create<string>();
                prereg2.Blog = blog;

                blog.Posts.Add(post);
                blog.Followers.Add(prereg2);

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

                var projector = PropertyProjectorFactory<Blog>.Create();
                var projection = projector
                    .Select(m => m.Author, m => m.Location)
                    .Include<Post>(p => p.Text, p => p.Date)
                    .Include<Follower>(p => p.Name);

                rep.RetrieveById(createdBlog.Id, projection);
            }
        }

        [TestMethod]
        public void CanCreate4()
        {
            using (var uow = UoWFactory.Create())
            {
                var blog = new Blog();
                blog.Author = _fixture.Create<string>();

                var post = new Post();
                post.Text = _fixture.Create<string>();
                blog.Posts.Add(post);

                var repository = uow.Create();

                var updatedBlog = repository.CreateGraph(blog);
                uow.Commit();
            }
        }
    }
}