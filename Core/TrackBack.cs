namespace Core
{
    public class TrackBack : IEntity
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string ReferrerUrl { get; set; }

        public string PostUrl { get; set; }

        public Post Post { get; set; }
    }
}