using Application.Features.Lessons.Dtos;

namespace Application.Features.Lessons.Queries.GetLessonOverview
{
    public sealed record GetLessonOverviewQuery(int CourseId, int ModuleId, int ItemId) : IRequest<OneOf<LessonOverviewDto, Error>>;
}
