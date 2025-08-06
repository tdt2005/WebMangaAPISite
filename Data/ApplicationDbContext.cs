using Microsoft.EntityFrameworkCore;
using MangaAPI.Models;

namespace MangaAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Reader> Readers { get; set; }
        public DbSet<Manga> Mangas { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure composite primary key for Follow.
            modelBuilder.Entity<Follow>().HasKey(f => new { f.ReaderID, f.MangaID });
        }
    }
}
