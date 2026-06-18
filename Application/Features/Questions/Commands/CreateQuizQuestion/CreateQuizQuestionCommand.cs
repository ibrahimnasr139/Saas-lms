using Domain.Enums;

namespace Application.Features.Questions.Commands.CreateQuizQuestion
{
    public sealed record CreateQuizQuestionCommand(int CourseId, int ModuleId, int ItemId, string? CorrectAnswer,
        Difficulty Difficulty, int? Category, string Question, QuestionType Type, List<QuestionOption>? Options, int Marks,
        int Order, bool RequiresManualGrading) : IRequest<OneOf<bool, Error>>;

}