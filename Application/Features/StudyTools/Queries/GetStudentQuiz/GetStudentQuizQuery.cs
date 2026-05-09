using Application.Features.StudyTools.Dtos;

namespace Application.Features.StudyTools.Queries.GetStudentQuiz
{
    public sealed record GetStudentQuizQuery(int QuizId) : IRequest<OneOf<StudentQuizDto, Error>>;
}