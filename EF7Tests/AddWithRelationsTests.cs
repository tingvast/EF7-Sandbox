﻿using Core;
using DataAccess.Interaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;
using System;
using System.Linq;

namespace EF7Tests
{
    [TestClass]
    public class AddWithRelationsTests : TestBase
    {
        [TestMethod]
        public void CanAddWithRelations()
        {
            #region Arrange

            var blog = new Blog();
            blog.Name = Fixture.Create<string>();

            var post = new Post();
            post.Text = Fixture.Create<string>();
            post.Url = Guid.NewGuid().ToString();
            blog.Posts.Add(post);

            #endregion Arrange

            #region Act

            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedBlog = repository.AddWithRelations(blog);
                uow.Commit();
            }

            #endregion Act

            #region Assert

            // TODO!

            #endregion Assert
        }

        [TestMethod]
        public void CanAddWithRelations2()
        {
            #region Arrange

            Blog createdBlog;
            Post post;

            var blog = new Blog
            {
                Name = Fixture.Create<string>(),
                Description = Fixture.Create<string>()
            };

            post = new Post()
            {
                Text = Fixture.Create<string>(),
                Date = Fixture.Create<string>(),
                Url = Guid.NewGuid().ToString(),
                Blog = blog,
            };

            var follower = new Follower()
            {
                Name = Fixture.Create<string>(),
                Blog = blog
            };

            blog.Posts.Add(post);
            blog.Followers.Add(follower);

            post = new Post();
            post.Text = Fixture.Create<string>();
            post.Date = Fixture.Create<string>();
            post.Url = Guid.NewGuid().ToString();
            post.Blog = blog;

            blog.Posts.Add(post);

            #endregion Arrange

            #region Act

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();

                createdBlog = rep.AddWithRelations(blog);

                uow.Commit();
            }

            #endregion Act

            #region Assert

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();

                var pp = rep.PropertySelectBuilder(blog)
                    .Select(m => m.Id, m => m.Name, m => m.Description)
                    .Include<Post>(p => p.Posts, p => p.Text, p => post.Date)
                    .Include<Follower>(m => m.Followers, p => p.Id, p => p.Name)
                    .Build();

                var retrievedBlog = rep.RetrieveById(createdBlog.Id, pp);
            }

            #endregion Assert
        }

        [TestMethod]
        public void CanAddWithRelations_Inconclusive()
        {
            Assert.Inconclusive("This implementation of retrieve is not going to be used");
            Post post;
            Blog createdBlog;
            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                var blog = new Blog
                {
                    Name = Fixture.Create<string>()
                };

                post = new Post();
                post.Text = Fixture.Create<string>();
                post.Date = Fixture.Create<string>();
                post.Url = Guid.NewGuid().ToString();
                post.Blog = blog;

                blog.Posts.Add(post);

                post = new Post();
                post.Text = Fixture.Create<string>();
                post.Date = Fixture.Create<string>();
                post.Url = Guid.NewGuid().ToString();
                post.Blog = blog;

                blog.Posts.Add(post);

                createdBlog = rep.AddWithRelations(blog);

                uow.Commit();
            }

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();

                var retrievedBlogWithPosts = rep.RetrieveObsolete<Blog, dynamic>(
                    createdBlog.Id, p => new { ff = p.Name, ffff = p.Description, fff = p.Posts.Select(pp => pp.Text) });
            }
        }

        [TestMethod]
        public void CanAddParentWithRelationsWhenSomePartsInGraphAlreadyAddedToRepository()
        {
            /*
            1. Create a blog, and add it to the repository. The parent object is added to repository (IsKeySet on entity will become true)
            2. Using a different contect add a post (navigation property entity) to the blog and add it to the repository.
            */

            #region Arrange

            Blog createdBlog;
            Post post;

            var blog = Fixture.Build<Blog>()
                .OmitAutoProperties()
                .With(p => p.Name, Fixture.Create<string>())
                .Create();

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();

                rep.AddWithRelations(blog);

                uow.Commit();
            }

            post = Fixture.Create<Post>();

            blog.Posts.Add(post);

            #endregion Arrange

            #region Act

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();

                createdBlog = rep.AddWithRelations(blog);

                uow.Commit();
            }

            #endregion Act

            #region Assert

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();

                var pp = rep.PropertySelectBuilder(blog)
                    .Select(m => m.Id, m => m.Name, m => m.Description)
                    .Include<Post>(p => p.Posts, p => p.Text, p => post.Date)
                    .Include<Follower>(m => m.Followers, p => p.Id, p => p.Name)
                    .Build();

                var retrievedBlog = rep.RetrieveById(createdBlog.Id, pp);
            }

            #endregion Assert
        }

        [TestMethod]
        public void CanPerformWhat()
        {
            //Assert.Inconclusive("This is unintuative!!");
            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();

                var blog = Fixture.Build<Blog>()
                    .OmitAutoProperties()
                    .With(p => p.Name, Fixture.Create<string>())
                    .Create();

                rep.Add(blog);

                var post = Fixture.Build<Post>()
                    .OmitAutoProperties()
                    .With(p => p.Url, Guid.NewGuid().ToString())
                    .With(p => p.Text, Fixture.Create<string>())
                    .Create();

                blog.Posts.Add(post);

                rep.AddWithRelations(blog);

                // The post will not be added to repository here, since blog entity is tracked by the first add
                // above. The AddWithRelations method used the TrackEntity method, which in turn stops traversing
                // the graph.

                uow.Commit();
            }
        }
    }
}