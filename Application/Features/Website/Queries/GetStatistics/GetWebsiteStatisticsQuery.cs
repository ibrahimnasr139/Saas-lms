using Application.Features.Website.Dtos;

namespace Application.Features.Website.Queries.GetStatistics
{
    public sealed record GetWebsiteStatisticsQuery : IRequest<OneOf<WebSiteStatisticsDto, Error>>;
}