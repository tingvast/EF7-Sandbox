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


                var kk = rep.Retrieve<Blog>(k.Id);
            }
        }

        [TestMethod]
        public void CanCreateGraph()
        {
            Post prereg;
            Blog createdMeeting;
            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                var meeting = new Blog
                {
                    Author = _fixture.Create<string>()
                };

                prereg = new Post();
                prereg.Text = _fixture.Create<string>();
                prereg.Date = _fixture.Create<string>();
                prereg.Blog = meeting;

                meeting.Posts.Add(prereg);

                prereg = new Post();
                prereg.Text = _fixture.Create<string>();
                prereg.Date = _fixture.Create<string>();
                prereg.Blog = meeting;

                meeting.Posts.Add(prereg);

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

                var retrievedMeetingWithPrereg = rep.Retrieve<Blog, dynamic>(
                      createdMeeting.Id, p => new { ff = p.Author, ffff = p.Location, fff = p.Posts.Select(pp => pp.Text) });

                //var retrievedMeetingWithPrereg = rep.Retrieve<Meeting, dynamic>(
                //          createdMeeting.Id, p => new { ff = p.Location });
            }
        }

        [TestMethod]
        public void CanCreateGraph2()
        {
            Post prereg;
            Blog createdMeeting;
            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                var meeting = new Blog
                {
                    Author = _fixture.Create<string>(),
                    Location = _fixture.Create<string>()
                };

                prereg = new Post();
                prereg.Text = _fixture.Create<string>();
                prereg.Date = _fixture.Create<string>();
                prereg.Blog = meeting;

                var prereg2 = new Follower();
                prereg2.Name = _fixture.Create<string>();
                prereg2.Blog = meeting;

                meeting.Posts.Add(prereg);
                meeting.Followers.Add(prereg2);

                prereg = new Post();
                prereg.Text = _fixture.Create<string>();
                prereg.Date = _fixture.Create<string>();
                prereg.Blog = meeting;

                meeting.Posts.Add(prereg);

                createdMeeting = rep.CreateGraph(meeting);

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

                rep.RetrieveById(createdMeeting.Id, projection);
            }
        }

        [TestMethod]
        public void CanRetrieveGraph()
        {
            Post prereg;
            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                var meeting = new Blog
                {
                    Author = "My Location",
                };

                prereg = new Post();
                prereg.Text = "First pre registration";
                prereg.Date = "Additional text";
                prereg.Blog = meeting;

                meeting.Posts.Add(prereg);

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
            Post prereg;
            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                prereg = new Post();
                prereg.Text = "First pre registration";
                prereg.Date = "Additional text";

                var k = rep.Create<Post>(prereg);

                uow.Commit();
            }

            var meeting = new Blog();
            meeting.Author = "Location 1";
            prereg.Text = "Update preregistratioon text";
            prereg.Blog = meeting;
            meeting.Posts.Add(prereg);

            var prereg1 = new Post();
            prereg1.Text = "New pre registratoob";
            prereg1.Blog = meeting;
            meeting.Posts.Add(prereg1);

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
                var meeting = new Blog();
                meeting.Author = "MyLocation1";

                var preReg = new Post();
                preReg.Text = "MyPre reg";
                //preReg.Meeting = meeting;
                meeting.Posts.Add(preReg);

                var repository = uow.Create();

                var updatedMeeting = repository.CreateGraph(meeting);
                uow.Commit();
            }
        }

        [TestMethod]
        public void CanCreate5()
        {
            var meeting = new Blog();
            meeting.Author = "kljljk";
            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedMeeting = repository.UpdateGraph(meeting);

                uow.Commit();
            }

            var preReg = new Post();
            preReg.Text = "lkajlkaj";

            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedMeeting = repository.UpdateGraph(preReg);

                uow.Commit();
            }

            meeting.Posts.Add(preReg);

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
            var meeting = new Blog();
            meeting.Author = "kljljk";
            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedMeeting = repository.Create(meeting);

                uow.Commit();
            }

            var preReg = new Post();
            preReg.Text = "lkajlkaj";

            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedMeeting = repository.Create(preReg);

                uow.Commit();
            }

            var newPreReg = new Post();
            newPreReg.Text = "kjlhkjhkjhkjhk";
            meeting.Posts.AddRange(new List<Post>() { preReg, newPreReg });

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
            var meeting = new Blog();
            meeting.Author = "kljljk";
            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedMeeting = repository.UpdateGraph(meeting);

                uow.Commit();
            }

            var preReg = new Post();
            preReg.Text = "lkajlkaj";

            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedMeeting = repository.UpdateGraph(preReg);

                uow.Commit();
            }

            var newPreReg = new Post();
            newPreReg.Text = "kjlhkjhkjhkjhk";
            meeting.Posts.AddRange(new List<Post>() { newPreReg, preReg });

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
            var meeting = new Blog();
            meeting.Author = "kljljk";
            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedMeeting = repository.Create(meeting);

                uow.Commit();
            }

            var preReg = new Post();
            preReg.Text = "lkajlkaj";

            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedMeeting = repository.Create(preReg);

                uow.Commit();
            }

            var newPreReg = new Post();
            newPreReg.Text = "kjlhkjhkjhkjhk";
            meeting.Posts.AddRange(new List<Post>() { newPreReg, preReg });

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

            var meeting = new Blog();
            meeting.Author = "kljljk";
            using (var uow = UoWFactory.Create())
            {
                var repository = uow.Create();

                var updatedMeeting = repository.UpdateGraph(meeting);

                uow.Commit();
            }

            #endregion Arrange

            #region Act

            meeting.Author = "new Location";

            using (var uow1 = UoWFactory.Create())
            {
                var repository = uow1.Create();

                var updatedMeeting = repository.Update(meeting, p => p.Author);

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