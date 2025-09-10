using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieCatalogAPI.Data;
using MovieCatalogAPI.Models;

namespace MovieCatalogAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly MovieContext _context;

        public MoviesController(MovieContext context)
        {
            _context = context;
        }

        // GET: api/movies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            return await _context.Movies.Include(m => m.Director).ToListAsync();
        }

        // GET: api/movies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovie(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.Director)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound($"Movie with ID {id} not found.");
            }

            return movie;
        }

        // POST: api/movies
        [HttpPost]
        public async Task<ActionResult<Movie>> CreateMovie(Movie movie)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if director exists
            var directorExists = await _context.Directors.AnyAsync(d => d.Id == movie.DirectorId);
            if (!directorExists)
            {
                return BadRequest($"Director with ID {movie.DirectorId} does not exist.");
            }

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
        }

        // PUT: api/movies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovie(int id, Movie movie)
        {
            if (id != movie.Id)
            {
                return BadRequest("Movie ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingMovie = await _context.Movies.FindAsync(id);
            if (existingMovie == null)
            {
                return NotFound($"Movie with ID {id} not found.");
            }

            // Check if director exists
            var directorExists = await _context.Directors.AnyAsync(d => d.Id == movie.DirectorId);
            if (!directorExists)
            {
                return BadRequest($"Director with ID {movie.DirectorId} does not exist.");
            }

            existingMovie.Title = movie.Title;
            existingMovie.DirectorId = movie.DirectorId;
            existingMovie.ReleaseYear = movie.ReleaseYear;
            existingMovie.Genre = movie.Genre;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("The movie was modified by another user.");
            }

            return NoContent();
        }

        // DELETE: api/movies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound($"Movie with ID {id} not found.");
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

}
