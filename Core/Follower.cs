namespace Core
{
    public class Follower : IEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Blog Blog { get; set; }
        public int? BlogId { get; set; }
    }
}