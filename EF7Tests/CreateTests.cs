using Core;
using DataAccess.Interaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;
using System.Data.SqlClient;

namespace EF7Tests
{
    /// <summary>
    /// Summary description for CreateTests
    /// </summary>
    [TestClass]
    public class CreateTests
    {
        private Fixture _fixture;

        public CreateTests()
        {
            _fixture = new Fixture();
        }

        [TestMethod]
        public void CanCreateBusinessObject()
        {
            Post prereg;
            using (var uow = UoWFactory.Create())
            {
                #region Arrange

                var rep = uow.Create();
                prereg = new Post();
                prereg.Text = _fixture.Create<string>();
                prereg.Date = _fixture.Create<string>();

                #endregion Arrange

                #region Act

                var persitedPreRegistration = rep.Create<Post>(prereg);

                uow.Commit();



                #endregion Act

                #region Assert

          //      Post retrievedPreRegistration = null;
          //      SqlConnection connection = new SqlConnection("Server = (localdb)\\ProjectsV12; Database = EF7; Trusted_Connection = true; MultipleActiveResultSets = True");
          //      SqlCommand command = new SqlCommand(
          //string.Format("SELECT Id, MeetingId, Text, Text1 FROM PreRegistration WHERE Id = {0};", persitedPreRegistration.Id),
          //connection);
          //      connection.Open();

          //      SqlDataReader reader = command.ExecuteReader();

          //      if (reader.HasRows)
          //      {
          //          retrievedPreRegistration = new Post();
          //          while (reader.Read())
          //          {
          //              retrievedPreRegistration.Id = reader.GetInt32(0);
          //              retrievedPreRegistration.BlogId = reader.IsDBNull(1) ? -1 : reader.GetInt32(1);
          //              retrievedPreRegistration.Text = reader.IsDBNull(2) ? "null" : reader.GetString(2);
          //              retrievedPreRegistration.Date = reader.IsDBNull(3) ? "null" : reader.GetString(3);
          //          }
          //      }

          //      Assert.IsNotNull(retrievedPreRegistration);
          //      Assert.AreEqual(persitedPreRegistration.Id, retrievedPreRegistration.Id);

                #endregion Assert
            }
        }

        [TestMethod]
        public void CanCreateAnotherBusinessObject()
        {
            Post prereg;
            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                var meeting = new Blog
                {
                    Author = _fixture.Create<string>()
                };

                var k = rep.Create<Blog>(meeting);

                uow.Commit();
            }
        }

       
    }
}