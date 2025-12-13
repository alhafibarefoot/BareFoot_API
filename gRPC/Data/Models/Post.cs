using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gRPC.Data.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(25)]
        public string? Title { get; set; }

        public string? Content { get; set; }

        [DefaultValue("Post.jfif")]
        public string? postImage { get; set; }

        [Column("CreatedOn", TypeName = "SmallDateTime")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedOn { get; set; }
    }
}
