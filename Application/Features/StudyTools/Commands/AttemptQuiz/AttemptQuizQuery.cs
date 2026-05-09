using Application.Features.StudyTools.Dtos;

namespace Application.Features.StudyTools.Commands.AttemptQuiz
{
    public sealed record AttemptQuizQuery(int QuizId, List<StudentAnswerDto> Answers, int CorrectAnswers, int IncorrectAnswers,
        bool Passed, int Score, int TimeSpent) : IRequest<OneOf<AttemptQuizDto, Error>>;
}