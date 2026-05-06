using Application.Features.TenantStudents.Dtos;

namespace Application.Features.TenantStudents.Commands.DeleteStudent
{
    public sealed record DeleteStudentCommand(int StudentId, int CourseId) : IRequest<OneOf<StudentResponse, Error>>;
}