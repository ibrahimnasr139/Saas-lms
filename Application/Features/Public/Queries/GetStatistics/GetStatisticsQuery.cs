using Application.Features.Public.Dtos;

namespace Application.Features.Public.Queries.GetStatistics
{
    public sealed record GetStatisticsQuery : IRequest<OneOf<PublicStatisticsDto, Error>>;
}