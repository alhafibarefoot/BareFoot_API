using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Swashbuckle.AspNetCore.Annotations;

namespace MinAPI.Data.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(25)]
        public string? Title { get; set; }

        [SwaggerParameter(Description = "Property description", Required = false)]
        public string? Content { get; set; }

        [DefaultValue("Post.jfif")]
        public string? postImage { get; set; }

        // public class Validator : AbstractValidator<Post>
        // {
        //     public Validator()
        //     {
        //         RuleFor(p => p.Title)
        //             .NotEmpty()
        //             .WithMessage("Title should not be Empty")
        //             .NotNull()
        //             .WithMessage("Title should not be Null")
        //             //.Length(6, 25)
        //             .MaximumLength(25)
        //             .WithMessage("Title Should not exceed 25 Character");
        //     }
        // }
    }
}
