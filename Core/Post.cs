namespace EF7
{
    public class Post : IEntity
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public string Date { get; set; }

        public Blog Blog { get; set; }
        public int? BlogId { get; set; }
    }
}