using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MangaAPI.Data;
using MangaAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMangaProject.DTO;
using MangaAPI.DTO;

namespace MangaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MangaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MangaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Manga
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Manga>>> GetAllMangas()
        {
            var allMangas = await _context.Mangas
                                          .Include(m => m.Chapters)
                                              .ThenInclude(c => c.Contents)
                                          .ToListAsync();
            return Ok(allMangas);
        }

        // GET: api/Manga/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Manga>> GetMangaById(string id)
        {
            var manga = await _context.Mangas
                                      .Include(m => m.Chapters)
                                          .ThenInclude(c => c.Contents)
                                      .FirstOrDefaultAsync(m => m.MangaID == id);

            if (manga == null)
                return NotFound();

            // Chỉ trả về các chapter thuộc Manga này
            var response = new
            {
                manga.MangaID,
                manga.Title,
                manga.Genres,
                manga.Thumbnails,
                manga.Descriptions,
                Chapters = manga.Chapters.Select(c => new
                {
                    c.ChapterID,
                    c.chapter_no,
                    ContentCount = c.Contents.Count // Đếm số nội dung trong mỗi chapter
                }).OrderBy(c => c.chapter_no) // Sắp xếp chương theo số chương
            };


            return Ok(manga);
        }

        // POST: api/Manga
        [HttpPost]
        public async Task<ActionResult<Manga>> PostManga(Manga manga)
        {
            _context.Mangas.Add(manga);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMangaById), new { id = manga.MangaID }, manga);
        }

        // PUT: api/Manga/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutManga(string id, Manga manga)
        {
            if (id != manga.MangaID)
            {
                return BadRequest();
            }

            _context.Entry(manga).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Mangas.Any(m => m.MangaID == id))
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

        // DELETE: api/Manga/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteManga(string id)
        {
            var manga = await _context.Mangas.FindAsync(id);
            if (manga == null)
            {
                return NotFound();
            }

            _context.Mangas.Remove(manga);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        // For Search Function
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<MangaDTO>>> SearchManga([FromQuery] string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest(new { message = "Search query cannot be empty" });
            }

            var mangas = await _context.Mangas
                .Where(m => EF.Functions.Like(m.Title, $"%{query}%"))
                .Select(m => new MangaDTO
                {
                    MangaID = m.MangaID,
                    Title = m.Title,
                    Thumbnails = m.Thumbnails,
                    Genres = m.Genres
                })
                .ToListAsync();

            return Ok(mangas);
        }
    }
}
