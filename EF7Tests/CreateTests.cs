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
            PreRegistration prereg;
            using (var uow = UoWFactory.Create())
            {
                #region Arrange

                var rep = uow.Create();
                prereg = new PreRegistration();
                prereg.Text = _fixture.Create<string>();
                prereg.Text1 = _fixture.Create<string>();

                #endregion Arrange

                #region Act

                var persitedPreRegistration = rep.Create<PreRegistration>(prereg);

                uow.Commit();

                #endregion Act

                #region Assert

                PreRegistration retrievedPreRegistration = null;
                SqlConnection connection = new SqlConnection("Server = (localdb)\\ProjectsV12; Database = EF7; Trusted_Connection = true; MultipleActiveResultSets = True");
                SqlCommand command = new SqlCommand(
          string.Format("SELECT Id, MeetingId, Text, Text1 FROM PreRegistration WHERE Id = {0};", persitedPreRegistration.Id),
          connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    retrievedPreRegistration = new PreRegistration();
                    while (reader.Read())
                    {
                        retrievedPreRegistration.Id = reader.GetInt32(0);
                        retrievedPreRegistration.MeetingId = reader.IsDBNull(1) ? -1 : reader.GetInt32(1);
                        retrievedPreRegistration.Text = reader.IsDBNull(2) ? "null" : reader.GetString(2);
                        retrievedPreRegistration.Text1 = reader.IsDBNull(3) ? "null" : reader.GetString(3);
                    }
                }

                Assert.IsNotNull(retrievedPreRegistration);
                Assert.AreEqual(persitedPreRegistration.Id, retrievedPreRegistration.Id);

                #endregion Assert
            }
        }

        [TestMethod]
        public void CanCreateAnotherBusinessObject()
        {
            PreRegistration prereg;
            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                var meeting = new Meeting
                {
                    Location = _fixture.Create<string>()
                };

                var k = rep.Create<Meeting>(meeting);

                uow.Commit();
            }
        }

       
    }
}