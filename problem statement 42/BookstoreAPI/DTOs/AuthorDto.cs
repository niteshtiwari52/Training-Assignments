namespace BookstoreAPI.DTOs
{
    public class AuthorDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Biography { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int BookCount { get; set; }
    }
}
