using System.ComponentModel.DataAnnotations;
using System.IO;

namespace MovieCatalogAPI.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public int DirectorId { get; set; }

        [Required]
        [Range(1900, 2030)]
        public int ReleaseYear { get; set; }

        public string? Genre { get; set; }

        // Navigation property
        public Director Director { get; set; }
    }
}
