using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MangaAPI.Models
{
    [Table("Manga")]
    public class Manga
    {
        [Key]
        public string MangaID { get; set; }
        public string Title { get; set; }
        public string Genres { get; set; }
        public string Thumbnails { get; set; }
        public string Descriptions { get; set; }

        // Navigation properties: initialize to avoid validation errors.
        public ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
        public ICollection<Follow> Followers { get; set; } = new List<Follow>();
    }
}
