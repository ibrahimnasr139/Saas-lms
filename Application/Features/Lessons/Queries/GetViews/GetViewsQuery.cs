using Application.Features.Lessons.Dtos;

namespace Application.Features.Lessons.Queries.GetViews
{
    public sealed record GetViewsQuery(int CourseId, int ModuleId, int ItemId) : IRequest<OneOf<IEnumerable<StudentViewsDto>, Error>>;
}
