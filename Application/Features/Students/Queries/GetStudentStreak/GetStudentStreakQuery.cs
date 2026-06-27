using Application.Features.Students.Dtos;

namespace Application.Features.Students.Queries.GetStudentStreak
{
    public sealed record GetStudentStreakQuery : IRequest<OneOf<StudentStreakDto, Error>>;
}