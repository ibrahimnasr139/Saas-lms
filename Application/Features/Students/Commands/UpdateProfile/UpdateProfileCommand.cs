using Application.Features.Students.Dtos;

namespace Application.Features.Students.Commands.UpdateProfile
{
    public sealed record UpdateProfileCommand(string? FirstName, string? LastName, string? Email, string? Phone, string? Avatar,
        string? Bio, string? Grade, string? Semester, string? Goal, List<StudentSubjectDto>? Subjects) : IRequest<OneOf<bool, Error>>;
}