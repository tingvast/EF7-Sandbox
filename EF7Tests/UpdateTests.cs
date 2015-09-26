using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;
using Core;
using DataAccess.Interaces;
using DataAccess;

namespace EF7Tests
{
    /// <summary>
    /// Summary description for UpdateTests
    /// </summary>
    [TestClass]
    public class UpdateTests
    {
        private Fixture _fixture;

        public UpdateTests()
        {
            _fixture = new Fixture();
        }

        [TestMethod]
        public void CanUpdateBusinessObject()
        {
            #region Arrange

            Post retrievedPost = null;
            using (var uow = UoWFactory.Create())
            {
                Post post;
                var rep = uow.Create();
                post = new Post();
                post.Text = _fixture.Create<string>();
                post.Date = _fixture.Create<string>();

                var persistedPost = rep.Create<Post>(post);

                uow.Commit();


                var projector = PropertyProjectorFactory<Post>.Create();
                projector
                    .Select(p => p.Text, p => p.Date);
                    
                retrievedPost = rep.RetrieveById(persistedPost.Id, projector);
            }

            #endregion Arrange

            #region Act
            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                retrievedPost.Text = _fixture.Create<string>();

                rep.Update(retrievedPost, p => p.Text);

                uow.Commit();
            }

            #endregion Act
        }
    }
}
