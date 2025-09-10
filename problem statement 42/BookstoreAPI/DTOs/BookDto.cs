using System.ComponentModel.DataAnnotations;

namespace BookstoreAPI.DTOs
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
    }

    // Create Book DTO
    public class CreateBookDto
    {
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
    }

    // update Book DTO
    public class UpdateBookDto
    {
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
    }
}
