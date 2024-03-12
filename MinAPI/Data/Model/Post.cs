using System.ComponentModel.DataAnnotations;

namespace MinAPI.Data.Model
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Title { get; set; }

        public string? Content { get; set; }

        public string? postImage { get; set; }
    }
}
