using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization; // or Newtonsoft.Json if using that

namespace MangaAPI.Models
{
    [Table("Comment")]
    public class Comment
    {
        [Key]
        public int CommentID { get; set; }

        [Required]
        public int ReaderID { get; set; }

        [Required]
        public string MangaID { get; set; }

        // Optional if you allow comments at the Manga level
        public string? ChapterID { get; set; }

        [Required]
        public string CommentText { get; set; }

        public DateTime CommentDate { get; set; } = DateTime.UtcNow;

        // Navigation properties: mark as nullable and ignore during JSON serialization/binding.
        [JsonIgnore]
        public virtual Reader? Reader { get; set; }

        [JsonIgnore]
        public virtual Manga? Manga { get; set; }

        [JsonIgnore]
        public virtual Chapter? Chapter { get; set; }
    }
}
