namespace LibraryManagement.Core.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public int PublishedYear { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
