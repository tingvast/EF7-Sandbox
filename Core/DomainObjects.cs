using EF7;
using System.Collections.Generic;

namespace Core
{
    public class Blog : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public List<Post> Posts { get; set; }

        public List<Follower> Followers { get; set; }

        public Blog()
        {
            Posts = new List<Post>();
            Followers = new List<Follower>();
        }
    }

    public class Post : IEntity
    {
        public int Id { get; set; }
        public string BlogText { get; set; }

        public string Date { get; set; }

        public Blog Blog { get; set; }
        public int? BlogId { get; set; }
    }

    public class Follower : IEntity
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; } 

        public Blog Blog { get; set; }
        public int? BlogId { get; set; }
    }
}