
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity.ChangeTracking;
using DataAccess.Interaces;
using ConsoleApplication3;

namespace EF7
{
    class EF7Program
    {
       

        static void Main(string[] args)
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

            //meeting.PreRegistrations.AddRange(new List<PreRegistration>() { prereg, prereg1 });
            //using (var uow = UoWFactory.Create())
            //{
            //    context.ChangeTracker.AttachGraph(meeting);
            //    prereg1.Text1 = "lkjalkjslkjslkjslkjlsj";
            //    meeting.PreRegistrations.Remove(prereg1);

            //    context.SaveChanges();
            //}

            //using (var context = new EF7BloggContext())
            //{
            //    context.Remove(prereg1);

            //    context.SaveChanges();
            //}
        }

        private static void target(EntityEntry obj)
        {
            obj.SetState(Microsoft.Data.Entity.EntityState.Added);
        }
    }
}
