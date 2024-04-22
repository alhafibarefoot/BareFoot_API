using FluentValidation;
using MinAPI.Data.Models;

namespace MinAPI.Validations
{
    public class PostValidator : AbstractValidator<Post>
    {
        public PostValidator()
        {
            RuleFor(p => p.Title)
                .NotEmpty()
                .WithMessage("Title should not be Empty")
                .NotNull()
                .WithMessage("Title should not be Null")
                //.Length(6, 25)
                .MaximumLength(25)
                .WithMessage("Title Should not exceed 25 Character");
        }
    }
}
