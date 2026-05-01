namespace Application.Features.Questions.Commands.DuplicateQuizQuestion
{
    public sealed record DuplicateQuizQuestionCommand(int CourseId, int ModuleId, int ItemId, int QuizQuestionId)
        : IRequest<OneOf<SuccessDto, Error>>;

}