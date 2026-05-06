using Application.Features.Quizzes.Dtos;

namespace Application.Features.Quizzes.Queries.GetPerformance
{
    public sealed record GetPerformanceQuery(int CourseId, int ModuleId, int ItemId) : IRequest<OneOf<PerformanceDto, Error>>;
}
