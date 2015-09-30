using DataAccess.Interaces;
using EF7;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;
using System.Collections.Generic;
using System.Linq;

namespace EF7Tests
{
    /// <summary>
    /// Summary description for Retrieve
    /// </summary>
    [TestClass]
    public class RetrieveTests : TestBase
    {
        [TestMethod]
        public void CanRetrieveBusinessObject()
        {
            using (var uow = UoWFactory.Create())
            {
                #region Arrange

                var rep = uow.Create();
                var blog = new Blog
                {
                    Name = Fixture.Create<string>()
                };

                var createdBlog = rep.Add<Blog>(blog);

                uow.Commit();

                #endregion Arrange

                #region Act

                //var retrievedBlog = rep.RetrieveById<Blog>(createdBlog.Id);

                #endregion Act

                #region Assert

                // TODO

                #endregion Assert
            }
        }

        [TestMethod]
        public void CanRetrieveByIdSelectedPropertiesFromBusinessObject()
        {
            using (var uow = UoWFactory.Create())
            {
                #region Arrange

                var rep = uow.Create();
                var blog = new Blog
                {
                    Name = Fixture.Create<string>()
                };

                var createdBlog = rep.Add<Blog>(blog);

                uow.Commit();

                #endregion Arrange

                #region Act

                var projector = rep.PropertySelectBuilder(blog)
                    .Select(p => p.Name)
                    .Include<Post>(m => m.Posts, p => p.Text)
                    .Build();

                var retrievedBlog = rep.RetrieveById<Blog>(createdBlog.Id, projector);

                #endregion Act

                #region Assert

                // TODO

                #endregion Assert
            }
        }

        [TestMethod]
        public void CanRetrieveBusinessObjectSelectedIntProperty()
        {
            #region Arrange

            Blog createdBlog;

            var blog = new Blog
            {
                Name = Fixture.Create<string>(),
                Description = Fixture.Create<string>()
            };

            #endregion Arrange

            #region Act

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();

                createdBlog = rep.Add(blog);

                uow.Commit();
            }

            #endregion Act

            #region Assert

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();

                var projector = rep.PropertySelectBuilder(blog)
                    .Select(p => p.Id)
                    .Build();

                var retrievedBlog = rep.RetrieveById(createdBlog.Id, projector);
            }

            #endregion Assert
        }

        [TestMethod]
        public void CanRetrieveBusinessObjectDiconnected()
        {
            Blog createdBlog = null;

            #region Arrange

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                var blog = new Blog
                {
                    Name = Fixture.Create<string>()
                };

                createdBlog = rep.Add<Blog>(blog);

                uow.Commit();
            }

            #endregion Arrange

            #region Act

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                //var retrievedBlog = rep.Retrieve<Blog>(createdBlog.Id);
            }

            #endregion Act
        }

        [TestMethod]
        public void CanRetrieveOrderBy()
        {
            #region Arrange

            string name = Fixture.Create<string>();
            List<Blog> blogs = new List<Blog>()
            {
                new Blog()
                {
                    Author = "Adam",
                    Description = Fixture.Create<string>(),
                    Name = name,
                },
                new Blog()
                {
                    Author = "Bertil",
                    Description = Fixture.Create<string>(),
                    Name = name,
                },
                new Blog()
                {
                    Author = "Cescar",
                    Description = Fixture.Create<string>(),
                    Name = name,
                }
            };

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();

                rep.Add(blogs.ToArray());

                uow.Commit();
            }

            #endregion Arrange

            #region Act

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();

                var retrieved = rep.Retrieve<Blog>(predicate => predicate.Name == name, order => order.Author).ToList();
            }

            #endregion Act
        }
    }
}