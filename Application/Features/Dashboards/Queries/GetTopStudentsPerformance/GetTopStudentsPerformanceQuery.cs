using Application.Features.Dashboards.Dtos;

namespace Application.Features.Dashboards.Queries.GetTopStudentsPerformance
{
    public sealed record GetTopStudentsPerformanceQuery : IRequest<OneOf<List<TopStudentsPerformanceDto>, Error>>;
}