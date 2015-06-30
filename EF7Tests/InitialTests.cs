using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleApplication3;
using DataAccess.Interaces;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EF7Tests
{
    [TestClass]
    public class InitialTests
    {

        public void SetUp()
        {
            
        }

        [TestMethod]
        public void CanCreate0()
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


                var retrieved = rep.Retrieve<PreRegistration, dynamic>(k.Id, p => new { p.Text });
            }


        }

        [TestMethod]
        public void CanCreate1()
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


                var retrieved = rep.Retrieve<PreRegistration, dynamic>(k.Id, p => new { p.Text });

                retrieved.Text1 = "lkaljla";

                uow.Commit();
            }


        }

        [TestMethod]
        public void CanCreate2()
        {
            PreRegistration prereg;
            using (var uow = UoWFactory.Create())
            {
                var rep = uow.Create();
                var meeting = new Meeting
                {
                    Location = "My Location",
                };

                var k = rep.Create<Meeting>(meeting);

                uow.Commit();              
            }


        }

        [TestMethod]
        public void CanCreateGraph()
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


                //var retrievedMeetingWithPrereg = rep.Retrieve<Meeting, dynamic>(
                //    createdMeeting.ID, p => new { p.Location, p.PreRegistrations });

                var r = rep.Retrieve(1, createdMeeting.Id);
                
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

                var updatedMeeting = repository.UpdateGraph(meeting);
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

            #endregion

            #region Act
            meeting.Location = "new Location";

            using (var uow1 = UoWFactory.Create())
            {
                var repository = uow1.Create();

                var updatedMeeting = repository.Update(meeting, p => p.Location);

                uow1.Commit();
            }

            #endregion

            #region Assert

            // Only Location should have been generated
            var uow2 = UoWFactory.Create();
            var r = uow2.Create();

            



            #endregion


        }
    }
}
