using System.ComponentModel.DataAnnotations;

namespace BookstoreAPI.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Author ID is required")]
        public int AuthorId { get; set; }

        [Required(ErrorMessage = "Publication year is required")]
        [Range(1000, 9999, ErrorMessage = "Publication year must be a valid 4-digit year")]
        public int PublicationYear { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value")]
        public decimal Price { get; set; }

        // Navigation property
        public Author? Author { get; set; }
    }
}
