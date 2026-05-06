using Application.Features.Questions.Dtos;

namespace Application.Features.Questions.Queries.GetStatistics
{
    public sealed record GetStatisticsQuery : IRequest<QuestionStatisticsDto>;
}
