using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieReview_API.Data;
using MovieReview_API.Models;

namespace MovieReview_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieReviewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MovieReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/MovieReviews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieReview>>> GetMovieReviews()
        {
            return await _context.MovieReviews.ToListAsync();
        }

        // GET: api/MovieReviews/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MovieReview>> GetMovieReview(int id)
        {
            var movieReview = await _context.MovieReviews.FindAsync(id);

            if (movieReview == null)
            {
                return NotFound();
            }

            return movieReview;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovieReview(int id, MovieReview movieReview)
        {
            if (id != movieReview.Id)
            {
                return BadRequest();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            // Ensure MovieId exists in the Movies table
            if (!_context.Movies.Any(m => m.MovieId == movieReview.MovieId))
            {
                return BadRequest("The specified MovieId does not exist.");
            }

            movieReview.UserId = userId;

            _context.Entry(movieReview).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieReviewExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool MovieReviewExists(int id)
        {
            return _context.MovieReviews.Any(e => e.Id == id);
        }

        // POST: api/MovieReviews
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<MovieReview>> PostMovieReview(MovieReview movieReview)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            movieReview.UserId = userId;
            movieReview.ReviewDate = DateTime.Now;

            _context.MovieReviews.Add(movieReview);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMovieReview", new { id = movieReview.Id }, movieReview);
        }

        // DELETE: api/MovieReviews/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovieReview(int id)
        {
            var movieReview = await _context.MovieReviews.FindAsync(id);
            if (movieReview == null)
            {
                return NotFound();
            }

            _context.MovieReviews.Remove(movieReview);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
