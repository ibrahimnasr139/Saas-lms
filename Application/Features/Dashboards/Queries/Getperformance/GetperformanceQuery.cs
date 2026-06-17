using Application.Features.Dashboards.Dtos;

namespace Application.Features.Dashboards.Queries.Getperformance
{
    public sealed record GetperformanceQuery : IRequest<OneOf<DashboardPerformanceDto, Error>>;
}