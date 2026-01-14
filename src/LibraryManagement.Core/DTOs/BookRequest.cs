namespace LibraryManagement.Core.DTOs
{
    public class BookRequest
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public int PublishedYear { get; set; }
    }
}
