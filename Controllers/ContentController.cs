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
    public class ContentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Content/{id?}
        [HttpGet("{id?}")]
        public async Task<ActionResult> GetContent(int? id)
        {
            if (id == null)
            {
                // Return all content
                var allContents = await _context.Contents.Include(c => c.Chapter).ToListAsync();
                return Ok(allContents);
            }
            else
            {
                // Return specific content by ID
                var content = await _context.Contents
                                            .Include(c => c.Chapter)
                                            .FirstOrDefaultAsync(c => c.ContentID == id.Value);
                if (content == null)
                    return NotFound();

                return Ok(content);
            }
        }

        // POST: api/Content
        [HttpPost]
        public async Task<ActionResult<Content>> PostContent(Content content)
        {
            _context.Contents.Add(content);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetContent), new { id = content.ContentID }, content);
        }

        // PUT: api/Content/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContent(int id, Content content)
        {
            if (id != content.ContentID)
            {
                return BadRequest();
            }

            _context.Entry(content).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Contents.Any(c => c.ContentID == id))
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

        // DELETE: api/Content/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContent(int id)
        {
            var content = await _context.Contents.FindAsync(id);
            if (content == null)
            {
                return NotFound();
            }

            _context.Contents.Remove(content);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
