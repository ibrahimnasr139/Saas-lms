using Application.Features.StudentQuizes.Dtos;

namespace Application.Features.StudentQuizes.Commands.SubmitQuiz
{
    public sealed record SubmitQuizCommand(int CourseId, int ItemId, List<QuizAnswerDto> Answers, int TimeSpent)
        : IRequest<OneOf<StudentQuizResponse, Error>>;
}