using System;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new BloggingContext())
            {
                var post = new Post() { Content = "jahgj", Title = "akjha" };
                var blog = new Blog { Url = "http://blogs.msdn.com/adonet" , Posts = new System.Collections.Generic.List<Post>() { post } };
                post.Blog = blog;

                db.ChangeTracker.TrackGraph(blog, (n) => n.State = Microsoft.Data.Entity.EntityState.Added);
                db.SaveChanges();

                foreach (var blog1 in db.Blogs)
                {
                    Console.WriteLine(blog1.Url);
                }
            }
        }
    }
}
