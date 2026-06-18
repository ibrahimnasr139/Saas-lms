using Application.Features.Website.Dtos;

namespace Application.Features.Website.Queries.GetTenantOrderStatistics
{
    public sealed record GetTenantOrderStatisticsQuery : IRequest<TenantOrderStatisticsDto>;
}
