using FluentValidation;

namespace Application.Features.StudentLessons.Commands.UpdateStudentLessonProgress
{
    internal sealed class UpdateStudentLessonProgressCommandValidator : AbstractValidator<UpdateStudentLessonProgressCommand>
    {
        public UpdateStudentLessonProgressCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .GreaterThan(0);

            RuleFor(x => x.ItemId)
                .GreaterThan(0);

            RuleFor(x => x.VideoId)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Duration)
                .GreaterThan(0);

            RuleFor(x => x.LastPosition)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x)
                .Must(x => x.LastPosition <= x.Duration)
                .WithMessage("Last position must be less than or equal duration.");

            RuleFor(x => x.Segments)
                .NotNull();

            RuleForEach(x => x.Segments)
                .Must(x => x is { Length: 2 })
                .WithMessage("Each segment must contain start and end.");
        }
    }
}