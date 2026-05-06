using Application.Features.Lessons.Dtos;

namespace Application.Features.Lessons.Queries.GetLessonPerformance
{
    public sealed record GetLessonPerformanceQuery(int CourseId, int ModuleId, int ItemId) : IRequest<OneOf<LessonPerformanceDto, Error>>;

}
