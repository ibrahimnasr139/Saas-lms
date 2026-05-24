using Application.Features.Lessons.Dtos;

namespace Application.Features.Lessons.Queries.GetLessonContent
{
    public sealed record GetLessonContentQuery(int CourseId, int ModuleId, int ItemId) : IRequest<OneOf<LessonContentDto, Error>>;
}