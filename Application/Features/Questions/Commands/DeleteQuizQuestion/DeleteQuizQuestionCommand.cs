namespace Application.Features.Questions.Commands.DeleteQuizQuestion
{
    public sealed record DeleteQuizQuestionCommand(int CourseId, int ModuleId, int ItemId, int QuestionId)
        : IRequest<OneOf<SuccessDto, Error>>;
}