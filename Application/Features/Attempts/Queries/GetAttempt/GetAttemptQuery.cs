using Application.Features.Attempts.Dtos;

namespace Application.Features.Attempts.Queries.GetAttempt
{
    public sealed record GetAttemptQuery(int QuizId, int AttemptId) : IRequest<OneOf<AttemptResponse, Error>>;
}
