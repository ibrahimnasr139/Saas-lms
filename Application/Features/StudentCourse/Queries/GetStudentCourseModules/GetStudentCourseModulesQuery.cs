using Application.Features.StudentCourse.Dtos;

namespace Application.Features.StudentCourse.Queries.GetStudentCourseModules
{
    public sealed record GetStudentCourseModulesQuery(int CourseId) : IRequest<OneOf<List<StudentModuleDto>, Error>>;
}