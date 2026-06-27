using Application.Features.Students.Commands.UpdateProfile;
using Application.Features.Students.Dtos;
using Application.Features.TenantStudents.Dtos;

namespace Application.Contracts.Repositories
{
    public interface IStudentRepository
    {
        Task<IEnumerable<string>> GetStudentsEmails(IEnumerable<int> studentIds, string subdomain, CancellationToken cancellationToken);
        Task CreateStudentAsync(Student student, CancellationToken cancellationToken);
        Task<Student?> GetStudentAsync(int studentId, CancellationToken cancellationToken);
        Task<int> GetStudentIdAsync(string userId, CancellationToken cancellationToken);
        Task<List<StudentsDto>> GetStudentsAsync(string subDomain, CancellationToken cancellationToken, int? courseId = null);
        Task<StudentStatisticsDto> GetStudentStatisticsAsync(string subDomain, CancellationToken cancellationToken);
        Task<bool> DeleteStudentAsync(int studentId, int courseId, CancellationToken cancellationToken);
        Task<StudentDto?> GetTenantStudentAsync(int studentId, string subDomain, CancellationToken cancellationToken);
        Task<List<AvailableSubjectDto>> GetAvailableSubjectsAsync(CancellationToken cancellationToken);
        Task UpdateHasOnboardedAsync(string userId, CancellationToken cancellationToken);
        Task<string> GetStudentEmailAsync(string userId, CancellationToken cancellationToken);
        Task<string> GetStuentNameByIdAsync(int studentId, CancellationToken cancellationToken);
        Task<Student?> GetStudentByInviteCodeAsync(string inviteCode, CancellationToken cancellationToken);
        Task<Student> GetStudentByIdAsync(int studentId, CancellationToken cancellationToken);
        Task<CurrentStudentDto> GetCurrentStudentAsync(string userId, int studentId, CancellationToken cancellationToken);
        Task UpdateStudentXPAsync(int studentId, int xpGained, CancellationToken cancellationToken);
        Task<string?> GetStudentGradeAsync(int studentId, CancellationToken cancellationToken);
        Task<StudentUserProfileDto> GetUserProfileAsync(string userId, string role, CancellationToken cancellationToken);
        Task<bool> UpdateUserProfileAsync(int studentId, UpdateProfileCommand request, CancellationToken cancellationToken);
        Task<ProfileDetailsDto> GetProfileDetailsAsync(int studentId, CancellationToken cancellationToken);
    }
}