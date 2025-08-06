using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MangaAPI.Data;
using MangaAPI.Models;

namespace MangaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Helper method to get the destination time zone.
        private TimeZoneInfo GetDestinationTimeZone(string? userTimeZoneId)
        {
            TimeZoneInfo destinationTimeZone = TimeZoneInfo.Local; // default to server's local time zone

            if (!string.IsNullOrEmpty(userTimeZoneId))
            {
                try
                {
                    destinationTimeZone = TimeZoneInfo.FindSystemTimeZoneById(userTimeZoneId);
                }
                catch (TimeZoneNotFoundException)
                {
                    // If the provided time zone is not found, fallback to server local time.
                    destinationTimeZone = TimeZoneInfo.Local;
                }
                catch (InvalidTimeZoneException)
                {
                    destinationTimeZone = TimeZoneInfo.Local;
                }
            }
            return destinationTimeZone;
        }

        // GET: api/comment
        // Returns all comments if no parameter is provided sorted by time,
        // return all comment in the chapter if chapterID is provided sorted by time,
        // return all comment of the Manga sort by chapter then time if only MangaID is provided,
        // and converts CommentDate from UTC to the user's time zone (or server's local time if not provided).
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComments(
    [FromQuery] string? mangaID,
    [FromQuery] string? chapterID,
    [FromQuery] string? userTimeZoneId)
        {
            IQueryable<Comment> query = _context.Comments;

            // If chapterID is provided (even if mangaID is also provided), filter by chapterID.
            if (!string.IsNullOrEmpty(chapterID))
            {
                query = query
                    .Where(c => c.ChapterID == chapterID)
                    .OrderByDescending(c => c.CommentDate);
            }
            // If only mangaID is provided, filter by mangaID.
            else if (!string.IsNullOrEmpty(mangaID))
            {
                query = query
                    .Where(c => c.MangaID == mangaID)
                    .OrderBy(c => c.ChapterID)
                    .ThenByDescending(c => c.CommentDate);
            }
            // If both mangaID and chapterID are null, then load all comments.
            else
            {
                query = query.OrderByDescending(c => c.CommentDate);
            }

            var comments = await query.ToListAsync();

            // Convert each comment's UTC CommentDate to the desired time zone.
            var destinationTimeZone = GetDestinationTimeZone(userTimeZoneId);
            comments.ForEach(comment =>
            {
                comment.CommentDate = TimeZoneInfo.ConvertTimeFromUtc(comment.CommentDate, destinationTimeZone);
            });

            return Ok(comments);
        }

        // GET: api/comment/5
        // Returns a single comment by its CommentID and converts CommentDate from UTC to the user's time zone (or server's local time if not provided).
        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(int id, [FromQuery] string? userTimeZoneId)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            var destinationTimeZone = GetDestinationTimeZone(userTimeZoneId);
            comment.CommentDate = TimeZoneInfo.ConvertTimeFromUtc(comment.CommentDate, destinationTimeZone);

            return comment;
        }

        // POST: api/comment
        [HttpPost]
        public async Task<ActionResult<Comment>> PostComment([FromBody] Comment comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetComment),
                new { id = comment.CommentID },
                comment
            );
        }

        // PUT: api/comment/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment(int id, [FromBody] Comment comment)
        {
            if (id != comment.CommentID)
            {
                return BadRequest("Comment ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(comment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Comments.Any(e => e.CommentID == id))
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

        // DELETE: api/comment/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
