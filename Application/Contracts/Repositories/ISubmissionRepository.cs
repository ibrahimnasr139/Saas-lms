using Application.Features.Assignments.Dtos;

namespace Application.Contracts.Repositories
{
    public interface ISubmissionRepository
    {
        Task<bool> IsSubmissionFound(int submissionId, int itemId, CancellationToken cancellationToken);
        Task GradeSubmission(int submissionId, double grade, string? feedback, CancellationToken cancellationToken);
        Task<List<SubmissionOverTime>> GetSubmissionsOverTimeAsync(int itemId, CancellationToken cancellationToken);
        Task<List<GradeDistribution>> GetSubmissionGradeDistributionAsync(int itemId, int totalMarks, CancellationToken cancellationToken);
    }
}
