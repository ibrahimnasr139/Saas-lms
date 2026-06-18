using FluentValidation;

namespace Application.Features.Website.Commands.CreateTenantPage
{
    public sealed class CreateTenantPageCommandValidator : AbstractValidator<CreateTenantPageCommand>
    {
        public CreateTenantPageCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

            RuleFor(x => x.Url)
                .NotEmpty().WithMessage("URL is required.")
                .MaximumLength(200).WithMessage("URL must not exceed 200 characters.")
                .Matches(@"^[a-zA-Z0-9\-_\/]+$")
                .WithMessage("URL may only contain letters, numbers, '-', '_', or '/'.");

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Invalid status value.");

            RuleFor(x => x.MetaTitle)
                .MaximumLength(200).WithMessage("Meta Title must not exceed 200 characters.")
                .When(x => !string.IsNullOrEmpty(x.MetaTitle));

            RuleFor(x => x.MetaDescription)
                .MaximumLength(500).WithMessage("Meta Description must not exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.MetaDescription));

            RuleFor(x => x.PageBlocks)
                .NotNull().WithMessage("Page content is required.")
                .Must(blocks => blocks.Count > 0)
                .WithMessage("At least one block is required for the page.");
        }
    }
}
