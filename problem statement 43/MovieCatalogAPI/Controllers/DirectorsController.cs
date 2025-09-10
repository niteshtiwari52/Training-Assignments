using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieCatalogAPI.Data;
using MovieCatalogAPI.Models;

namespace MovieCatalogAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DirectorsController : ControllerBase
    {
        private readonly MovieContext _context;

        public DirectorsController(MovieContext context)
        {
            _context = context;
        }

        // GET: api/directors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Director>>> GetDirectors()
        {
            return await _context.Directors.Include(d => d.Movies).ToListAsync();
        }

        // GET: api/directors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Director>> GetDirector(int id)
        {
            var director = await _context.Directors
                .Include(d => d.Movies)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (director == null)
            {
                return NotFound($"Director with ID {id} not found.");
            }

            return director;
        }

        // GET: api/directors/5/movies
        [HttpGet("{directorId}/movies")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMoviesByDirector(int directorId)
        {
            var director = await _context.Directors.FindAsync(directorId);
            if (director == null)
            {
                return NotFound($"Director with ID {directorId} not found.");
            }

            var movies = await _context.Movies
                .Where(m => m.DirectorId == directorId)
                .Include(m => m.Director)
                .ToListAsync();

            return movies;
        }

        // POST: api/directors
        [HttpPost]
        public async Task<ActionResult<Director>> CreateDirector(Director director)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Directors.Add(director);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDirector), new { id = director.Id }, director);
        }

        // PUT: api/directors/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDirector(int id, Director director)
        {
            if (id != director.Id)
            {
                return BadRequest("Director ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingDirector = await _context.Directors.FindAsync(id);
            if (existingDirector == null)
            {
                return NotFound($"Director with ID {id} not found.");
            }

            existingDirector.Name = director.Name;
            existingDirector.BirthYear = director.BirthYear;
            existingDirector.Nationality = director.Nationality;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("The director was modified by another user.");
            }

            return NoContent();
        }

        // DELETE: api/directors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDirector(int id)
        {
            var director = await _context.Directors.FindAsync(id);
            if (director == null)
            {
                return NotFound($"Director with ID {id} not found.");
            }

            // Check if director has movies
            var hasMovies = await _context.Movies.AnyAsync(m => m.DirectorId == id);
            if (hasMovies)
            {
                return BadRequest("Cannot delete director who has movies associated.");
            }

            _context.Directors.Remove(director);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
