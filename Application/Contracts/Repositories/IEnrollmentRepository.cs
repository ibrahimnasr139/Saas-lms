using Application.Features.StudentCourse.Dtos;

namespace Application.Contracts.Repositories
{
    public interface IEnrollmentRepository
    {
        Task CreateEnrollmentAsync(Enrollment enrollment, CancellationToken cancellationToken);
        Task<bool> StudentIsAlreadyEnrolledAsync(int studentId, int courseId, CancellationToken cancellationToken);
        Task<List<string>> GetEmailsByCourseIdsAsync(int[] courseIds, CancellationToken cancellationToken);
        Task<List<string>> GetAllStudentEmailsAsync(int tenantId, CancellationToken cancellationToken);
        Task<List<StudentCoursesDto>> GetStudentCoursesAsync(int studentId, CancellationToken cancellationToken);
        Task<StudentCourseDto?> GetStudentCourseAsync(int studentId, int courseId, CancellationToken cancellationToken);
        Task<List<StudentModuleDto>> GetStudentCourseModulesAsync(int studentId, int courseId, CancellationToken cancellationToken);
        Task<int> GetTenantIdAsync(int studentId, int courseId, CancellationToken cancellationToken);
    }
}