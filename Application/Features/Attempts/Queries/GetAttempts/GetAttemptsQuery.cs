using Application.Features.Attempts.Dtos;

namespace Application.Features.Quizzes.Queries.GetAttempts
{
    public sealed record GetAttemptsQuery(int CourseId, int ModuleId, int ItemId) : IRequest<OneOf<IEnumerable<AttemptDto>, Error>>;
}
