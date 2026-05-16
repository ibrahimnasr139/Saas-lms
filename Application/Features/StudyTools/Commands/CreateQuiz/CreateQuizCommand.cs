using Application.Features.StudyTools.Dtos;
using Domain.Enums;

namespace Application.Features.StudyTools.Commands.CreateQuiz
{
    public sealed record CreateQuizCommand(int SubjectId, int NumberOfQuestions, StudentQuizDifficulty Difficulty, int? ChapterId,
        string Topic) : IRequest<OneOf<CreateQuizDto, Error>>;
}