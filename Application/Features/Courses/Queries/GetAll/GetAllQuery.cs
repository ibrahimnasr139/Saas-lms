using Application.Features.Courses.Dtos;
using Domain.Enums;

namespace Application.Features.Courses.Queries.GetAll
{
    public sealed record GetAllQuery(string? Q, int? GradeId, int? SubjectId, string? SortBy, string? SortOrder, CourseStatus? Status, int? Cursor, string? LastSortValue)
        : IRequest<AllCoursesDto>;
}
