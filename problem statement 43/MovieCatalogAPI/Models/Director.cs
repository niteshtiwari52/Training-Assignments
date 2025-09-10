using System.ComponentModel.DataAnnotations;

namespace MovieCatalogAPI.Models
{
    public class Director
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public int? BirthYear { get; set; }

        public string? Nationality { get; set; }

        // Navigation property
        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}
