namespace Application.Features.Questions.Commands.ImportQuizQuestion
{
    public sealed record ImportQuizQuestionCommand(int CourseId, int ModuleId, int ItemId, IEnumerable<int> QuestionIds)
        : IRequest<OneOf<bool, Error>>;
}