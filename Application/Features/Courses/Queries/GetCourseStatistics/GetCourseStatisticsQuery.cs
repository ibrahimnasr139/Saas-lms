using Application.Features.Courses.Dtos;

namespace Application.Features.Courses.Queries.GetCourseStatistics
{
    public sealed record GetCourseStatisticsQuery(int CourseId) : IRequest<OneOf<CourseStatisticsDto, Error>>;
}
