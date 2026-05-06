using Application.Features.Assignments.Dtos;

namespace Application.Features.Assignments.Queries.GetOverview
{
    public sealed record GetOverviewQuery(int CourseId, int ModuleId, int ItemId) : IRequest<OneOf<OverviewDto, Error>>;
}