using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MangaAPI.Data;
using MangaAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using WebMangaProject.DTO;

namespace MangaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReaderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReaderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Reader
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reader>>> GetReaders()
        {
            return await _context.Readers.ToListAsync();
        }

        // GET: api/Reader/{id?}
        [HttpGet("{id?}")]
        public async Task<ActionResult> GetReader(int? id)
        {
            // If no id is provided, return all readers
            if (id == null)
            {
                var allReaders = await _context.Readers.ToListAsync();
                return Ok(allReaders);
            }

            // If id is provided, return that specific reader
            var reader = await _context.Readers.FindAsync(id.Value);
            if (reader == null)
                return NotFound();

            return Ok(reader);
        }

        // POST: api/Reader
        [HttpPost]
        public async Task<ActionResult<Reader>> PostReader(Reader reader)
        {
            // 1. Hash the plain-text password before saving
            if (!string.IsNullOrEmpty(reader.Password))
            {
                reader.Password = BCrypt.Net.BCrypt.HashPassword(reader.Password);
            }

            // 2. Add the new reader to the database
            _context.Readers.Add(reader);
            await _context.SaveChangesAsync();

            // 3. Return the created reader (including the auto-generated ID)
            return CreatedAtAction(nameof(GetReader), new { id = reader.ReaderID }, reader);
        }

        // PUT: api/Reader/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReader(int id, Reader reader)
        {
            if (id != reader.ReaderID)
            {
                return BadRequest();
            }

            // If you also want to re-hash a changed password, do it here
            // if (!string.IsNullOrEmpty(reader.Password)) 
            // {
            //     reader.Password = BCrypt.Net.BCrypt.HashPassword(reader.Password);
            // }

            _context.Entry(reader).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Readers.Any(r => r.ReaderID == id))
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

        // DELETE: api/Reader/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReader(int id)
        {
            var reader = await _context.Readers.FindAsync(id);
            if (reader == null)
            {
                return NotFound();
            }

            _context.Readers.Remove(reader);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
