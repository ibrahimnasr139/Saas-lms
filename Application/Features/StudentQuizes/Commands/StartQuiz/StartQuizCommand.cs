using Application.Features.StudentQuizes.Dtos;

namespace Application.Features.StudentQuizes.Commands.StartQuiz
{
    public sealed record StartQuizCommand(int CourseId, int ItemId) : IRequest<OneOf<StudentQuizResponse, Error>>;
}