using BookstoreAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BookstoreAPI.Data
{
    public class BookstoreContext : DbContext
    {
        public BookstoreContext(DbContextOptions<BookstoreContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId);

            // Seed data
            modelBuilder.Entity<Author>().HasData(
                new Author { Id = 1, Name = "J.K. Rowling", Biography = "British author, best known for the Harry Potter series", DateOfBirth = new DateTime(1965, 7, 31) },
                new Author { Id = 2, Name = "George Orwell", Biography = "English novelist and essayist", DateOfBirth = new DateTime(1903, 6, 25) },
                new Author { Id = 3, Name = "Jane Austen", Biography = "English novelist known for her wit and social commentary", DateOfBirth = new DateTime(1775, 12, 16) }
            );

            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "Harry Potter and the Philosopher's Stone", AuthorId = 1, PublicationYear = 1997, Description = "The first book in the Harry Potter series", Price = 12.99m },
                new Book { Id = 2, Title = "1984", AuthorId = 2, PublicationYear = 1949, Description = "A dystopian social science fiction novel", Price = 10.99m },
                new Book { Id = 3, Title = "Pride and Prejudice", AuthorId = 3, PublicationYear = 1813, Description = "A romantic novel of manners", Price = 9.99m }
            );
        }
    }
}