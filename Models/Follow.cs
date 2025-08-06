using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MangaAPI.Models
{
    [Table("Follow")]
    public class Follow
    {
        // Composite primary key is configured in the DbContext.
        public int ReaderID { get; set; }
        public string MangaID { get; set; }

        // Navigation properties.
        [ValidateNever]
        [JsonIgnore]
        public Reader Reader { get; set; }

        [ValidateNever]
        [JsonIgnore]
        public Manga Manga { get; set; }
    }
}
