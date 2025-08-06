using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MangaAPI.Models
{
    [Table("Content")]
    public class Content
    {
        [Key]
        public int ContentID { get; set; }

        [ForeignKey("Chapter")]
        public string ChapterID { get; set; }

        public int Image_no { get; set; }

        [Required]
        public string Image_path { get; set; }

        // Navigation property
        [JsonIgnore]
        [ValidateNever]
        public Chapter Chapter { get; set; }
    }
}
