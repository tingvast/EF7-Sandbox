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


                var kk = rep.Retrieve<Meeting>(k.Id);
            }
        }

        [TestMethod]
        public void CanCreateGraph()
        {
            PreRegistration prereg;
            Meeting createdMeeting;
            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                var meeting = new Meeting
                {
                    Location = _fixture.Create<string>()
                };

                prereg = new PreRegistration();
                prereg.Text = _fixture.Create<string>();
                prereg.Text1 = _fixture.Create<string>();
                prereg.Meeting = meeting;

                meeting.PreRegistrations.Add(prereg);

                prereg = new PreRegistration();
                prereg.Text = _fixture.Create<string>();
                prereg.Text1 = _fixture.Create<string>();
                prereg.Meeting = meeting;

                meeting.PreRegistrations.Add(prereg);

                createdMeeting = rep.CreateGraph(meeting);

                uow.Commit();

                



                //var retrievedMeetingWithPrereg = rep.Retrieve<Meeting, dynamic>(
                //    createdMeeting.Id, p => new { p.Location, p.PreRegistrations });

                //var r = rep.Retrieve(1, createdMeeting.Id);
            }

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();

                //var retrievedMeetingWithPrereg = rep.Retrieve<Meeting, dynamic>(
                //  createdMeeting.Id, p => new { p.Location, p.PreRegistrations });
                //var retrievedMeetingWithPrereg = rep.Retrieve(0, createdMeeting.Id);

                //var retrievedMeetingWithPrereg = rep.Retrieve<Meeting, dynamic>(
                //  createdMeeting.Id, p => new { ff = p.Location, fff = (from ggg in p.PreRegistrations where ggg.Id == createdMeeting.Id select new { ggg.Text }).ToList() });

                var retrievedMeetingWithPrereg = rep.Retrieve<Meeting, dynamic>(
                      createdMeeting.Id, p => new { ff = p.Location, ffff = p.Location1, fff = p.PreRegistrations.Select(pp => pp.Text) });

                //var retrievedMeetingWithPrereg = rep.Retrieve<Meeting, dynamic>(
                //          createdMeeting.Id, p => new { ff = p.Location });
            }
        }

        [TestMethod]
        public void CanCreateGraph2()
        {
            PreRegistration prereg;
            Meeting createdMeeting;
            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                var meeting = new Meeting
                {
                    Location = _fixture.Create<string>()
                };

                prereg = new PreRegistration();
                prereg.Text = _fixture.Create<string>();
                prereg.Text1 = _fixture.Create<string>();
                prereg.Meeting = meeting;

                meeting.PreRegistrations.Add(prereg);

                prereg = new PreRegistration();
                prereg.Text = _fixture.Create<string>();
                prereg.Text1 = _fixture.Create<string>();
                prereg.Meeting = meeting;

                meeting.PreRegistrations.Add(prereg);

                createdMeeting = rep.CreateGraph(meeting);

                uow.Commit();

            }

            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();

                var projector = PropertyProjectorFactory<Meeting>.Create();
                var projection = projector
                    .Select(m => m.Location, m => m.Location1)
                    .Include<PreRegistration>(p => p.Text, p => p.Text1);

                rep.RetrieveById(createdMeeting.Id, projection);
            }
        }

        [TestMethod]
        public void CanRetrieveGraph()
        {
            PreRegistration prereg;
            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                var meeting = new Meeting
                {
                    Location = "My Location",
                };

                prereg = new PreRegistration();
                prereg.Text = "First pre registration";
                prereg.Text1 = "Additional text";
                prereg.Meeting = meeting;

                meeting.PreRegistrations.Add(prereg);

                var createdMeeting = rep.CreateGraph(meeting);

                uow.Commit();

                var r = rep.Retrieve(1, createdMeeting.Id);
                //var retrievedMeetingWithPrereg = rep.Retrieve<Meeting, dynamic>(
                //    createdMeeting.Id, p => new { p.PreRegistrations });
            }
        }

        [TestMethod]
        public void CanCreate3()
        {
            PreRegistration prereg;
            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                prereg = new PreRegistration();
                prereg.Text = "First pre registration";
                prereg.Text1 = "Additional text";

                var k = rep.Create<PreRegistration>(prereg);

                uow.Commit();
            }

            var meeting = new Meeting();
            meeting.Location = "Location 1";
            prereg.Text = "Update preregistratioon text";
            prereg.Meeting = meeting;
            meeting.PreRegistrations.Add(prereg);

            var prereg1 = new PreRegistration();
            prereg1.Text = "New pre registratoob";
            prereg1.Meeting = meeting;
            meeting.PreRegistrations.Add(prereg1);

            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedMeeting = repository.UpdateGraph(meeting);
                uow.Commit();
            }
        }

        [TestMethod]
        public void CanCreate4()
        {
            using (var uow = UoWFactory.Create())
            {
                var meeting = new Meeting();
                meeting.Location = "MyLocation1";

                var preReg = new PreRegistration();
                preReg.Text = "MyPre reg";
                //preReg.Meeting = meeting;
                meeting.PreRegistrations.Add(preReg);

                var repository = uow.Create();

                var updatedMeeting = repository.CreateGraph(meeting);
                uow.Commit();
            }
        }

        [TestMethod]
        public void CanCreate5()
        {
            var meeting = new Meeting();
            meeting.Location = "kljljk";
            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedMeeting = repository.UpdateGraph(meeting);

                uow.Commit();
            }

            var preReg = new PreRegistration();
            preReg.Text = "lkajlkaj";

            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedMeeting = repository.UpdateGraph(preReg);

                uow.Commit();
            }

            meeting.PreRegistrations.Add(preReg);

            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedMeeting = repository.UpdateGraph(meeting);

                uow.Commit();
            }
        }

        [TestMethod]
        public void CanCreate6()
        {
            var meeting = new Meeting();
            meeting.Location = "kljljk";
            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedMeeting = repository.Create(meeting);

                uow.Commit();
            }

            var preReg = new PreRegistration();
            preReg.Text = "lkajlkaj";

            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedMeeting = repository.Create(preReg);

                uow.Commit();
            }

            var newPreReg = new PreRegistration();
            newPreReg.Text = "kjlhkjhkjhkjhk";
            meeting.PreRegistrations.AddRange(new List<PreRegistration>() { preReg, newPreReg });

            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedMeeting = repository.UpdateGraph(meeting);

                uow.Commit();
            }
        }

        [TestMethod]
        public void CanCreate7()
        {
            var meeting = new Meeting();
            meeting.Location = "kljljk";
            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedMeeting = repository.UpdateGraph(meeting);

                uow.Commit();
            }

            var preReg = new PreRegistration();
            preReg.Text = "lkajlkaj";

            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedMeeting = repository.UpdateGraph(preReg);

                uow.Commit();
            }

            var newPreReg = new PreRegistration();
            newPreReg.Text = "kjlhkjhkjhkjhk";
            meeting.PreRegistrations.AddRange(new List<PreRegistration>() { newPreReg, preReg });

            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedMeeting = repository.UpdateGraph(meeting);

                uow.Commit();
            }
        }

        [TestMethod]
        public void CanCreate8()
        {
            var meeting = new Meeting();
            meeting.Location = "kljljk";
            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedMeeting = repository.Create(meeting);

                uow.Commit();
            }

            var preReg = new PreRegistration();
            preReg.Text = "lkajlkaj";

            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedMeeting = repository.Create(preReg);

                uow.Commit();
            }

            var newPreReg = new PreRegistration();
            newPreReg.Text = "kjlhkjhkjhkjhk";
            meeting.PreRegistrations.AddRange(new List<PreRegistration>() { newPreReg, preReg });

            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedMeeting = repository.UpdateGraph(meeting);

                uow.Commit();
            }

            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                repository.Delete(meeting);

                uow.Commit();
            }
        }

        [TestMethod]
        public void CanUpdateSelectedProperties()
        {
            #region Arrange

            var meeting = new Meeting();
            meeting.Location = "kljljk";
            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedMeeting = repository.UpdateGraph(meeting);

                uow.Commit();
            }

            #endregion Arrange

            #region Act

            meeting.Location = "new Location";

            using (var uow1 = UoWFactory.Create())
            {
                var repository = uow1.Create();

                var updatedMeeting = repository.Update(meeting, p => p.Location);

                uow1.Commit();
            }

            #endregion Act

            #region Assert

            // Only Location should have been generated
            var uow2 = UoWFactory.Create();
            var r = uow2.Create();

            #endregion Assert
        }
    }
}