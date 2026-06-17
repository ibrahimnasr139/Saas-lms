using Application.Features.Dashboards.Dtos;

namespace Application.Features.Dashboards.Queries.GetStatistics
{
    public sealed record GetStatisticsQuery : IRequest<OneOf<DashboardStatisticsDto, Error>>;
}