using Application.Features.Courses.Dtos;

namespace Application.Features.Courses.Queries.GetStatistics
{
    public sealed record GetStatisticsQuery : IRequest<StatisticsDto>;
}
