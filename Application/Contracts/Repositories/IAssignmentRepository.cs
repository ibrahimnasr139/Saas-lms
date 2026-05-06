using Application.Features.Assignments.Dtos;
using Application.Features.StudentAssignments.Dtos;

namespace Application.Contracts.Repositories
{
    public interface IAssignmentRepository
    {
        Task<List<StudentSubmissionDto>> GetSubmissionsAsync(int courseId, int itemId, CancellationToken cancellationToken);
        Task<OverviewDto?> GetOverviewAsync(int itemId, int courseId, CancellationToken cancellationToken);
        Task CreateAssignmentSubmissionAsync(AssignmentSubmission assignmentSubmission, CancellationToken cancellationToken);
        Task<AssignmentDto?> GetAssignmentAsync(int itemId, int courseId, CancellationToken cancellationToken);
        Task<AssignmentSubmissionDto?> GetStudentSubmissionAsync(int studentId, int itemId, CancellationToken cancellationToken);
    }
}