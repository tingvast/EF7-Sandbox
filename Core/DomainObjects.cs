﻿using EF7;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Blog : IEntity
    {
        public int Id { get; set; }
        public string Author { get; set; }

        public string Location { get; set; }
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
        public string Text { get; set; }

        public string Date { get; set; }

        public Blog Blog { get; set; }
        public int? BlogId { get; set; }
    }

    public class Follower : IEntity
    {
        public int Id { get; set; }

        public string  Name { get; set; }

        public Blog Blog { get; set; }
        public int? BlogId { get; set; }
    }
}
