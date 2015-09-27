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
    /// Summary description for UpdateGraphTests
    /// </summary>
    [TestClass]
    public class UpdateGraphTests
    {
        private Fixture _fixture;

        public UpdateGraphTests()
        {
            _fixture = new Fixture();
        }

        [TestMethod]
        public void CanCreate3()
        {
            Post post;
            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                post = new Post();
                post.Text = _fixture.Create<string>();
                post.Date = _fixture.Create<string>();

                var k = rep.Create<Post>(post);

                uow.Commit();
            }

            var blog = new Blog();
            blog.Author = _fixture.Create<string>();
            post.Text = _fixture.Create<string>();
            post.Blog = blog;
            blog.Posts.Add(post);

            post = new Post();
            post.Text = _fixture.Create<string>();
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
            blog.Author = _fixture.Create<string>();
            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedBlog = repository.UpdateGraph(blog);

                uow.Commit();
            }

            var post = new Post();
            post.Text = _fixture.Create<string>();

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
            blog.Author = _fixture.Create<string>();
            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var persistedBlog = repository.Create(blog);

                uow.Commit();
            }

            var post = new Post();
            post.Text = _fixture.Create<string>();

            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var persistedPost = repository.Create(post);

                uow.Commit();
            }

            var post1 = new Post();
            post1.Text = _fixture.Create<string>();
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
            blog.Author = _fixture.Create<string>();
            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedBlog = repository.UpdateGraph(blog);

                uow.Commit();
            }

            var post = new Post();
            post.Text = _fixture.Create<string>();

            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedBlog = repository.UpdateGraph(post);

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
        }
    }
}
