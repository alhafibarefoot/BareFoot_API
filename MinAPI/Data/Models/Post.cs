using System.ComponentModel.DataAnnotations;

namespace MinAPI.Data.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(25)]
        public string? Title { get; set; }

        public string? Content { get; set; }

        public string? postImage { get; set; }
    }
}
