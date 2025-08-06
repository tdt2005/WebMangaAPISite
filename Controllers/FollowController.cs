using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MangaAPI.Data;
using MangaAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMangaProject.DTO;

namespace MangaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FollowController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Follow/{readerId?}/{mangaId?}
        [HttpGet]
        public async Task<ActionResult> GetFollow([FromQuery] int? readerId, [FromQuery] string mangaId = null)
        {
            // 1) No parameters => return all
            if (readerId == null && string.IsNullOrEmpty(mangaId))
            {
                var allFollows = await _context.Follows
                    .Include(f => f.Reader)
                    .Include(f => f.Manga)
                    .Select(f => new FollowDTO
                    {
                        ReaderID = f.ReaderID,
                        MangaID = f.MangaID,
                        MangaTitle = f.Manga.Title,
                        MangaThumbnail = f.Manga.Thumbnails
                    })
                    .ToListAsync();
                return Ok(allFollows);
            }

            // 2) If both are supplied => return the specific follow
            if (readerId != null && !string.IsNullOrEmpty(mangaId))
            {
                var follow = await _context.Follows
                    .Include(f => f.Reader)
                    .Include(f => f.Manga)
                    .Where(f => f.ReaderID == readerId && f.MangaID == mangaId)
                    .Select(f => new FollowDTO
                    {
                        ReaderID = f.ReaderID,
                        MangaID = f.MangaID,
                        MangaTitle = f.Manga.Title,
                        MangaThumbnail = f.Manga.Thumbnails
                    })
                    .FirstOrDefaultAsync();
                if (follow == null)
                    return NotFound();
                return Ok(follow);
            }

            // 3) If only readerId is supplied => return all Follows for that Reader
            if (readerId != null && string.IsNullOrEmpty(mangaId))
            {
                var followsForReader = await _context.Follows
                    .Include(f => f.Reader)
                    .Include(f => f.Manga)
                    .Where(f => f.ReaderID == readerId)
                    .Select(f => new FollowDTO
                    {
                        ReaderID = f.ReaderID,
                        MangaID = f.MangaID,
                        MangaTitle = f.Manga.Title,
                        MangaThumbnail = f.Manga.Thumbnails
                    })
                    .ToListAsync();
                return Ok(followsForReader);
            }

            // 4) If only mangaId is supplied => return all Follows for that Manga
            if (readerId == null && !string.IsNullOrEmpty(mangaId))
            {
                var followsForManga = await _context.Follows
                    .Include(f => f.Reader)
                    .Include(f => f.Manga)
                    .Where(f => f.MangaID == mangaId)
                    .Select(f => new FollowDTO
                    {
                        ReaderID = f.ReaderID,
                        MangaID = f.MangaID,
                        MangaTitle = f.Manga.Title,
                        MangaThumbnail = f.Manga.Thumbnails
                    })
                    .ToListAsync();
                return Ok(followsForManga);
            }

            // Shouldn't get here logically, but just in case:
            return BadRequest("Invalid combination of parameters.");
        }

        // POST: api/Follow
        [HttpPost]
        public async Task<ActionResult<Follow>> PostFollow(Follow follow)
        {
            _context.Follows.Add(follow);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetFollow), new { readerId = follow.ReaderID, mangaId = follow.MangaID }, follow);
        }

        // DELETE: api/Follow/{readerId}/{mangaId}
        [HttpDelete("{readerId}/{mangaId}")]
        public async Task<IActionResult> DeleteFollow(int readerId, string mangaId)
        {
            var follow = await _context.Follows.FindAsync(readerId, mangaId);
            if (follow == null)
            {
                return NotFound();
            }

            _context.Follows.Remove(follow);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
