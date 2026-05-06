using Application.Features.Attempts.Dtos;

namespace Application.Contracts.Repositories
{
    public interface IAnswerRepository
    {
        Task UpdateTeacherScore(int attemptId, List<QuestionDto> questions, CancellationToken cancellationToken);
    }
}
