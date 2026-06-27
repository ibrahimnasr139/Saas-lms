using FluentValidation;

namespace Application.Features.Students.Commands.Onboarding
{
    public sealed class OnboardingCommandValidator : AbstractValidator<OnboardingCommand>
    {
        public OnboardingCommandValidator()
        {
            RuleFor(x => x.Grade)
                .NotEmpty().WithMessage("Grade is required")
                .MaximumLength(100);

            RuleFor(x => x.Semester)
                .NotEmpty().WithMessage("Semester is required")
                .MaximumLength(50);

            RuleFor(x => x.Goal)
                .NotEmpty().WithMessage("Goal is required")
                .MaximumLength(250);

            RuleFor(x => x.Subjects)
                .NotNull().WithMessage("Subjects list is required")
                .NotEmpty().WithMessage("At least one subject is required");

            RuleForEach(x => x.Subjects)
                .NotEmpty().WithMessage("Subject key cannot be empty")
                .MaximumLength(100);

            RuleFor(x => x.Confidence)
                .NotNull().WithMessage("Confidence dictionary is required")
                .NotEmpty().WithMessage("Confidence must contain values");

            RuleFor(x => x)
                .Must(x => x.Subjects.All(s => x.Confidence.ContainsKey(s)))
                .WithMessage("Each subject must have a confidence value");

            RuleFor(x => x)
                .Must(x => x.Confidence.Keys.All(k => x.Subjects.Contains(k)))
                .WithMessage("Confidence contains invalid subject keys");

            RuleForEach(x => x.Confidence)
                .Must(kv => kv.Value >= 0 && kv.Value <= 100)
                .WithMessage("Confidence must be between 0 and 100");

            RuleFor(x => x.Subjects)
                .Must(subjects => subjects.Distinct().Count() == subjects.Count)
                .WithMessage("Duplicate subjects are not allowed");
        }
    }
}