using Application.Features.Dashboards.Dtos;

namespace Application.Features.Dashboards.Queries.GetQuickAnalytics
{
    public sealed record GetQuickAnalyticsQuery : IRequest<OneOf<QuickAnalyticsDto, Error>>;
}