using FluentValidation;
namespace Application.Features.Students.Commands.UpdateProfile
{
    public sealed class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
    {
        public UpdateProfileCommandValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.Name));

            RuleFor(x => x.Email)
                .EmailAddress()
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.Phone)
                .MaximumLength(20)
                .Matches(@"^(?:\+201[0-2,5]\d{8}|01[0-2,5]\d{8})$")
                .When(x => !string.IsNullOrWhiteSpace(x.Phone));

            RuleFor(x => x.Avatar)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.Avatar));

            RuleFor(x => x.Bio)
                .MaximumLength(1000)
                .When(x => !string.IsNullOrWhiteSpace(x.Bio));

            RuleFor(x => x.Grade)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.Grade));

            RuleFor(x => x.Semester)
                .MaximumLength(50)
                .When(x => !string.IsNullOrWhiteSpace(x.Semester));

            RuleFor(x => x.Goal)
                .MaximumLength(250)
                .When(x => !string.IsNullOrWhiteSpace(x.Goal));

            RuleFor(x => x.Subjects)
                .Must(x => x == null || x.Count > 0)
                .WithMessage("Subjects must contain at least one subject if provided.");
        }
    }
}
