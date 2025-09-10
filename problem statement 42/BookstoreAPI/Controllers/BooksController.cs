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
    public class BooksController : ControllerBase
    {
        private readonly BookstoreContext _context;

        public BooksController(BookstoreContext context)
        {
            _context = context;
        }

        // GET: api/books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
        {
            var books = await _context.Books
                .Include(b => b.Author)
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

        // GET: api/books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBook(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return NotFound(new { message = $"Book with ID {id} not found" });
            }

            var bookDto = new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                AuthorId = book.AuthorId,
                AuthorName = book.Author!.Name,
                PublicationYear = book.PublicationYear,
                Description = book.Description,
                Price = book.Price
            };

            return Ok(bookDto);
        }

        // POST: api/books
        [HttpPost]
        public async Task<ActionResult<BookDto>> CreateBook(CreateBookDto createBookDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if author exists
            var authorExists = await _context.Authors.AnyAsync(a => a.Id == createBookDto.AuthorId);
            if (!authorExists)
            {
                return BadRequest(new { message = $"Author with ID {createBookDto.AuthorId} not found" });
            }

            var book = new Book
            {
                Title = createBookDto.Title,
                AuthorId = createBookDto.AuthorId,
                PublicationYear = createBookDto.PublicationYear,
                Description = createBookDto.Description,
                Price = createBookDto.Price
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            // Fetch the created book with author information
            var createdBook = await _context.Books
                .Include(b => b.Author)
                .FirstAsync(b => b.Id == book.Id);

            var bookDto = new BookDto
            {
                Id = createdBook.Id,
                Title = createdBook.Title,
                AuthorId = createdBook.AuthorId,
                AuthorName = createdBook.Author!.Name,
                PublicationYear = createdBook.PublicationYear,
                Description = createdBook.Description,
                Price = createdBook.Price
            };

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, bookDto);
        }

        // PUT: api/books/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, UpdateBookDto updateBookDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound(new { message = $"Book with ID {id} not found" });
            }

            // Check if author exists
            var authorExists = await _context.Authors.AnyAsync(a => a.Id == updateBookDto.AuthorId);
            if (!authorExists)
            {
                return BadRequest(new { message = $"Author with ID {updateBookDto.AuthorId} not found" });
            }

            book.Title = updateBookDto.Title;
            book.AuthorId = updateBookDto.AuthorId;
            book.PublicationYear = updateBookDto.PublicationYear;
            book.Description = updateBookDto.Description;
            book.Price = updateBookDto.Price;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound(new { message = $"Book with ID {id} not found" });
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
