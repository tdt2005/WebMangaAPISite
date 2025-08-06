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
    public class ChapterController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ChapterController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/Chapter/{id?}
        [HttpGet("{id?}")]
        public async Task<ActionResult> GetChapter(string id = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                // Return all chapters with associated manga and their contents
                var allChapters = await _context.Chapters
                                                .Include(c => c.Manga)
                                                .Include(c => c.Contents)
                                                .ToListAsync();
                return Ok(allChapters);
            }
            else
            {
                // Return a specific chapter with its associated manga and contents
                var chapter = await _context.Chapters
                                            .Include(c => c.Manga)
                                            .Include(c => c.Contents)
                                            .FirstOrDefaultAsync(c => c.ChapterID == id);
                if (chapter == null)
                    return NotFound();

                return Ok(chapter);
            }
        }

        // POST: api/Chapter
        [HttpPost]
        public async Task<ActionResult<Chapter>> PostChapter(Chapter chapter)
        {
            _context.Chapters.Add(chapter);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetChapter), new { id = chapter.ChapterID }, chapter);
        }

        // PUT: api/Chapter/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChapter(string id, Chapter chapter)
        {
            if (id != chapter.ChapterID)
            {
                return BadRequest();
            }

            _context.Entry(chapter).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Chapters.Any(c => c.ChapterID == id))
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

        // DELETE: api/Chapter/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChapter(string id)
        {
            var chapter = await _context.Chapters.FindAsync(id);
            if (chapter == null)
            {
                return NotFound();
            }

            _context.Chapters.Remove(chapter);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/Chapter/manga/{mangaID}
        [HttpGet("manga/{mangaID}")]
        public async Task<ActionResult> GetChaptersByManga(string mangaID)
        {
            var chapters = await _context.Chapters
                                         .Where(c => c.MangaID == mangaID)
                                         .OrderBy(c => c.chapter_no)
                                         .Select(c => new
                                         {
                                             c.ChapterID,
                                             c.chapter_no,
                                             ContentCount = c.Contents.Count // Đếm số ảnh trong chương
                                         })
                                         .ToListAsync();

            if (!chapters.Any())
                return NotFound();

            return Ok(chapters);
        }

    }
}
