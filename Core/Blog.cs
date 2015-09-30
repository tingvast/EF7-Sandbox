using System.Collections.Generic;

namespace EF7
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

        // TODO Adde behavior here!!
    }
}