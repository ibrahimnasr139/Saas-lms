using Application.Features.StudentQuizes.Dtos;

namespace Application.Features.StudentQuizes.Queries
{
    public sealed record GetStudentQuizQuery(int CourseId, int ItemId) : IRequest<OneOf<StudentQuizDto, Error>>;
}