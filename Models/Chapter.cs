using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace MangaAPI.Models
{
    [Table("Chapter")]
    public class Chapter
    {
        [Key]
        public string ChapterID { get; set; }

        [ForeignKey("Manga")]
        public string MangaID { get; set; }
        public double chapter_no { get; set; }

        // Navigation properties.
        [JsonIgnore]
        [ValidateNever]
        public Manga Manga { get; set; }
        public ICollection<Content> Contents { get; set; } = new List<Content>();
    }
}
