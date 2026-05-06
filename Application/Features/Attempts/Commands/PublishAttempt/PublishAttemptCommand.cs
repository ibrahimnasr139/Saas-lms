namespace Application.Features.Attempts.Commands.PublishAttempt
{
    public sealed record PublishAttemptCommand(int QuizId, int AttemptId)
        : IRequest<OneOf<SuccessDto, Error>>;
}
