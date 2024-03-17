using System.ComponentModel.DataAnnotations;

namespace MinAPI.Data.DTOs
{
    public class PostDTOs
    {
        public class PostReadDto
        {
            public int Id { get; set; }

            public string? Title { get; set; }

            public string? Content { get; set; }

            public string? postImage { get; set; }
        }

        public class PostNewOrUpdatedDto
        {
            [Key]
            public int Id { get; set; }

            [Required]
            [MinLength(3)]
            public string? Title { get; set; }

            public string? Content { get; set; }

            public string? postImage { get; set; }
        }
    }
}
