using Domain.Enums;

namespace Application.Features.Questions.Commands.UpdateQuestion
{
    public sealed record UpdateQuestionCommand(int QuestionId, string? CorrectAnswer, string? Explanation, Difficulty Difficulty, int Category, string Question,
        QuestionType Type, List<QuestionOption>? Options) : IRequest<OneOf<SuccessDto, Error>>;
}
