using Domain.Enums;

namespace Application.Features.Questions.Commands.CreateQuizQuestion
{
    public sealed record CreateQuizQuestionCommand(int CourseId, int ModuleId, int ItemId) : IRequest<OneOf<bool, Error>>
    {
        public string? CorrectAnswer { get; init; }
        public Difficulty Difficulty { get; init; }
        public int? Category { get; init; }
        public string Question { get; init; } = string.Empty;
        public QuestionType Type { get; init; }
        public List<QuestionOption>? Options { get; init; }
        public int Marks { get; init; }
        public int Order { get; init; }
        public bool RequiresManualGrading { get; init; }
    }
}