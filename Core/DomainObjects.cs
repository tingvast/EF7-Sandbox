using EF7;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3
{
    public class Meeting : IEntity
    {
        public int Id { get; set; }
        public string Location { get; set; }

        public string Location1 { get; set; }
        public List<PreRegistration> PreRegistrations { get; set; }

        public Meeting()
        {
            PreRegistrations = new List<PreRegistration>();
        }
    }

    public class PreRegistration : IEntity
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public string Text1 { get; set; }

        public Meeting Meeting{ get; set; }
        public int? MeetingId { get; set; }
    }
}
