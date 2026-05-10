using Application.Features.ModuleItems.Dtos;
using Application.Features.StudentCourse.Dtos;
using Domain.Enums;

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
        Task<List<NewModuleItemNotificationDto>> GetEnrolledStudentsForNotificationAsync(int courseId, string itemTitle, string itemType, DateTime? dueDate, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken);
    }
}