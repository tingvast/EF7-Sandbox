using Core;
using DataAccess;
using DataAccess.Interaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;
using System.Collections.Generic;
using System.Linq;

namespace EF7Tests
{
    [TestClass]
    public class InitialTests
    {
        private Fixture _fixture;


        public InitialTests()
        {
            _fixture = new Fixture();
        }
        public void SetUp()
        {
            //this.context.Database.EnsureDeleted();
            //this.context.Database.EnsureCreated();
        }

        [TestMethod]
        public void CanCreateAnotherBusinessObject11()
        {
            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                var blog = new Blog
                {
                    Author = _fixture.Create<string>()
                };

                var createdBlog = rep.Create<Blog>(blog);



                uow.Commit();


                var retrievedBlog = rep.Retrieve<Blog>(createdBlog.Id);
            }
        }

        [TestMethod]
        public void CanCreateGraph()
        {
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
        public void CanRetrieveGraph()
        {
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

                var createdBlog = rep.CreateGraph(blog);

                uow.Commit();

                var r = rep.Retrieve(1, createdBlog.Id);
            }
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

        [TestMethod]
        public void CanUpdateSelectedProperties()
        {
            #region Arrange

            var blog = new Blog();
            blog.Author = _fixture.Create<string>();
            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedBlog = repository.UpdateGraph(blog);

                uow.Commit();
            }

            #endregion Arrange

            #region Act

            blog.Author = _fixture.Create<string>();

            using (var uow1 = UoWFactory.Create())
            {
                var repository = uow1.Create();

                var updatedBlog = repository.Update(blog, p => p.Author);

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