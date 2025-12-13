using FluentValidation;
using MVCAPI.Data.Models;
using static MVCAPI.Data.DTOs.PostDTOs;

namespace MVCAPI.Validations
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

    public class PostNewOrUpdatedDtoValidator : AbstractValidator<PostNewOrUpdatedDto>
    {
        public PostNewOrUpdatedDtoValidator()
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
