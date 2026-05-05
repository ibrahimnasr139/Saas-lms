using Application.Features.Assignments.Dtos;

namespace Application.Features.Assignments.Queries.GetPerformance
{
    public sealed record GetPerformanceQuery(int CourseId, int ModuleId, int ItemId) : IRequest<OneOf<PerformanceDto, Error>>;
}