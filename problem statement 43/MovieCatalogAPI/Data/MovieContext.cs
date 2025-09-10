using Microsoft.EntityFrameworkCore;
using MovieCatalogAPI.Models;

namespace MovieCatalogAPI.Data
{
    public class MovieContext : DbContext
    {
        public MovieContext(DbContextOptions<MovieContext> options) : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Director> Directors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships
            modelBuilder.Entity<Movie>()
                .HasOne(m => m.Director)
                .WithMany(d => d.Movies)
                .HasForeignKey(m => m.DirectorId);

            // Seed data
            modelBuilder.Entity<Director>().HasData(
                new Director { Id = 1, Name = "Christopher Nolan", BirthYear = 1970, Nationality = "British-American" },
                new Director { Id = 2, Name = "Quentin Tarantino", BirthYear = 1963, Nationality = "American" },
                new Director { Id = 3, Name = "Martin Scorsese", BirthYear = 1942, Nationality = "American" }
            );

            modelBuilder.Entity<Movie>().HasData(
                new Movie { Id = 1, Title = "Inception", DirectorId = 1, ReleaseYear = 2010, Genre = "Sci-Fi" },
                new Movie { Id = 2, Title = "The Dark Knight", DirectorId = 1, ReleaseYear = 2008, Genre = "Action" },
                new Movie { Id = 3, Title = "Pulp Fiction", DirectorId = 2, ReleaseYear = 1994, Genre = "Crime" },
                new Movie { Id = 4, Title = "The Departed", DirectorId = 3, ReleaseYear = 2006, Genre = "Crime" }
            );
        }
    }
}
