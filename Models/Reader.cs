using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MangaAPI.Models
{
    [Table("Reader")]
    public class Reader
    {
        [Key]
        public int ReaderID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        // Initialize Follows to an empty list to avoid validation errors.
        public ICollection<Follow> Follows { get; set; } = new List<Follow>();
    }
}
