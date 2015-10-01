using System.Collections.Generic;

namespace Core
{
    public class Post : IEntity
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public string Url { get; set; }

        public string Date { get; set; }

        public Blog Blog { get; set; }
        public int? BlogId { get; set; }

        public List<TrackBack> TrackBacks { get; set; }
    }
}