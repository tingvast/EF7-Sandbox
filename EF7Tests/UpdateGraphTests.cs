using Core;
using DataAccess.Interaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;
using System.Collections.Generic;

namespace EF7Tests
{
    /// <summary>
    /// Summary description for UpdateGraphTests
    /// </summary>
    [TestClass]
    public class UpdateGraphTests : TestBase
    {
        [TestMethod]
        public void CanCreate3()
        {
            Post post;
            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                post = new Post();
                post.Text = Fixture.Create<string>();
                post.Date = Fixture.Create<string>();

                var k = rep.Add<Post>(post);

                uow.Commit();
            }

            var blog = new Blog();
            blog.Name = Fixture.Create<string>();
            post.Text = Fixture.Create<string>();
            post.Blog = blog;
            blog.Posts.Add(post);

            post = new Post();
            post.Text = Fixture.Create<string>();
            post.Blog = blog;
            blog.Posts.Add(post);

            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedBlog = repository.UpdateGraph(blog);
                uow.Commit();
            }
        }

        [TestMethod]
        public void CanCreate5()
        {
            var blog = new Blog();
            blog.Name = Fixture.Create<string>();
            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedBlog = repository.UpdateGraph(blog);

                uow.Commit();
            }

            var post = new Post();
            post.Text = Fixture.Create<string>();

            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedBlog = repository.UpdateGraph(post);

                uow.Commit();
            }

            blog.Posts.Add(post);

            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedBlog = repository.UpdateGraph(blog);

                uow.Commit();
            }
        }

        [TestMethod]
        public void CanCreate6()
        {
            var blog = new Blog();
            blog.Name = Fixture.Create<string>();
            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var persistedBlog = repository.Add(blog);

                uow.Commit();
            }

            var post = new Post();
            post.Text = Fixture.Create<string>();

            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var persistedPost = repository.Add(post);

                uow.Commit();
            }

            var post1 = new Post();
            post1.Text = Fixture.Create<string>();
            blog.Posts.AddRange(new List<Post>() { post, post1 });

            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedBlog = repository.UpdateGraph(blog);

                uow.Commit();
            }
        }

        [TestMethod]
        public void CanCreate7()
        {
            var blog = new Blog();
            blog.Name = Fixture.Create<string>();
            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedBlog = repository.UpdateGraph(blog);

                uow.Commit();
            }

            var post = new Post();
            post.Text = Fixture.Create<string>();

            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedBlog = repository.UpdateGraph(post);

                uow.Commit();
            }

            var newPost = new Post();
            newPost.Text = Fixture.Create<string>();
            blog.Posts.AddRange(new List<Post>() { newPost, post });

            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedBlog = repository.UpdateGraph(blog);

                uow.Commit();
            }
        }
    }
}