using Domain.Enums;

namespace Application.Features.Questions.Commands.UpdateQuizQuestion
{
    public sealed record UpdateQuizQuestionCommand(int CourseId, int ModuleId, int ItemId, int QuestionId, string? CorrectAnswer,
        Difficulty Difficulty, int Category, string Question, QuestionType Type, List<QuestionOption>? Options, int Marks,
        int Order, bool RequiresManualGrading) : IRequest<OneOf<bool, Error>>;

}