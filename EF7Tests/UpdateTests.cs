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

            Post retrieved = null;
            using (var uow = UoWFactory.Create())
            {
                Post prereg;
                var rep = uow.Create();
                prereg = new Post();
                prereg.Text = _fixture.Create<string>();
                prereg.Date = _fixture.Create<string>();

                var k = rep.Create<Post>(prereg);

                uow.Commit();

                retrieved = rep.Retrieve<Post, dynamic>(k.Id, p => new { p.Text });
            }

            #endregion Arrange

            #region Act
            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                retrieved.Text = _fixture.Create<string>();

                rep.Update(retrieved, p => p.Text);

                uow.Commit();
            }

            #endregion Act
        }
    }
}
