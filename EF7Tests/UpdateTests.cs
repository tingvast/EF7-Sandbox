﻿using System;
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

            PreRegistration retrieved = null;
            using (var uow = UoWFactory.Create())
            {
                PreRegistration prereg;
                var rep = uow.Create();
                prereg = new PreRegistration();
                prereg.Text = _fixture.Create<string>();
                prereg.Text1 = _fixture.Create<string>();

                var k = rep.Create<PreRegistration>(prereg);

                uow.Commit();

                retrieved = rep.Retrieve<PreRegistration, dynamic>(k.Id, p => new { p.Text });
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