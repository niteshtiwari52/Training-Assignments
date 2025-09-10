using BookstoreAPI.Data;
using BookstoreAPI.DTOs;
using BookstoreAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookstoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly BookstoreContext _context;

        public AuthorsController(BookstoreContext context)
        {
            _context = context;
        }

        // GET: api/authors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
        {
            return await _context.Authors.ToListAsync();
        }

        // GET: api/authors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Author>> GetAuthor(int id)
        {
            var author = await _context.Authors.FindAsync(id);

            if (author == null)
            {
                return NotFound(new { message = $"Author with ID {id} not found" });
            }

            return author;
        }

        // GET: api/authors/5/books
        [HttpGet("{authorId}/books")]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksByAuthor(int authorId)
        {
            var authorExists = await _context.Authors.AnyAsync(a => a.Id == authorId);
            if (!authorExists)
            {
                return NotFound(new { message = $"Author with ID {authorId} not found" });
            }

            var books = await _context.Books
                .Include(b => b.Author)
                .Where(b => b.AuthorId == authorId)
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    AuthorId = b.AuthorId,
                    AuthorName = b.Author!.Name,
                    PublicationYear = b.PublicationYear,
                    Description = b.Description,
                    Price = b.Price
                })
                .ToListAsync();

            return Ok(books);
        }
    }
}
