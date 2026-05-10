using Application.Features.Assignments.Dtos;
using Application.Features.StudentAssignments.Dtos;
using Domain.Enums;

namespace Infrastructure.Repositories
{
    internal sealed class AssignmentRepository : IAssignmentRepository
    {
        private readonly AppDbContext _dbContext;

        public AssignmentRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<OverviewDto?> GetOverviewAsync(int itemId, int courseId, CancellationToken cancellationToken)
        {
            return await _dbContext.AssignmentSubmissions.Where(s => s.AssignmentId == itemId)
                .GroupBy(s => 1)
                .Select(g => new OverviewDto
                {
                    TotalSubmissions = g.Count(),
                    TotalStudents = _dbContext.Students.Count(s => s.Enrollments.Any(e => e.CourseId == courseId)),
                    AverageScore = g.Average(s => s.EarnedMarks),
                    HighestScore = g.Max(s => s.EarnedMarks),
                    LowestScore = g.Min(s => s.EarnedMarks)
                }).FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<List<StudentSubmissionDto>> GetSubmissionsAsync(int courseId, int itemId, CancellationToken cancellationToken)
        {
            var totalMarks = await _dbContext.Assignments
                .Where(a => a.ModuleItemId == itemId)
                .Select(a => a.Marks)
                .FirstOrDefaultAsync(cancellationToken);

            return await _dbContext.Students.Where(s => s.Enrollments.Any(c => c.CourseId == courseId))
                 .LeftJoin(_dbContext.AssignmentSubmissions.Where(sv => sv.AssignmentId == itemId),
                     student => student.Id,
                     studentSubmission => studentSubmission.StudentId,
                     (student, studentSubmission) => new StudentSubmissionDto
                     {
                         Id = studentSubmission != null ? studentSubmission.Id : 0,
                         StudentName = student.User.FirstName + " " + student.User.LastName,
                         ProfilePicture = student.User.ProfilePicture,
                         Status = studentSubmission != null ? studentSubmission.Status : AssignmentStatus.NotSubmitted,
                         SubmittedAt = studentSubmission != null ? studentSubmission.SubmittedAt : null,
                         EarnedMarks = studentSubmission != null ? studentSubmission.EarnedMarks : null,
                         Feedback = studentSubmission != null ? studentSubmission.Feedback : null,
                         Link = studentSubmission != null ? studentSubmission.Link : null,
                         Text = studentSubmission != null ? studentSubmission.Text : null,
                         TotalMarks = totalMarks,
                         File = studentSubmission != null ? new FileDto
                         {
                             FileName = studentSubmission.File!.Name,
                             Url = studentSubmission.File.Url
                         } : null
                     }
                 ).ToListAsync(cancellationToken);
        }
        public async Task<AssignmentDto?> GetAssignmentAsync(int itemId, int courseId, CancellationToken cancellationToken)
        {
            var moduleItem = await _dbContext.ModuleItems
                .AsNoTracking()
                .Where(mi => mi.Id == itemId && mi.CourseId == courseId)
                .Select(mi => new AssignmentDto
                {
                    Title = mi.Title,
                    Description = mi.Description,
                    Instructions = mi.Assignment!.Instructions,
                    SubmissionType = mi.Assignment.SubmissionType,
                    DueDate = mi.Assignment.DueDate,
                    TotalMarks = mi.Assignment.Marks,
                    Attachments = mi.Assignment.Attachments,
                    CreatedAt = mi.Assignment.ModuleItem.CreatedAt,
                    UpdatedAt = mi.Assignment.ModuleItem.UpdatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);

            return moduleItem;
        }
        public async Task<AssignmentSubmissionDto?> GetStudentSubmissionAsync(int studentId, int itemId, CancellationToken cancellationToken)
        {
            var submission = await _dbContext.AssignmentSubmissions
                .AsNoTracking()
                .Where(s => s.StudentId == studentId && s.AssignmentId == itemId)
                .Select(s => new AssignmentSubmissionDto
                {
                    Id = s.Id,
                    AssignmentId = s.AssignmentId,
                    StudentId = s.StudentId,
                    Status = s.Status,
                    Score = (int)s.EarnedMarks,
                    Feedback = s.Feedback,
                    Link = s.Link,
                    Text = s.Text,
                    SubmittedAt = s.SubmittedAt,
                    SubmissionFiles = s.File == null ? null : new SubmissionFileDto
                    {
                        FileName = s.File.Name,
                        Url = s.File.Url
                    },
                    GradedBy = s.Student.StudentGrades.Select(x => x.GraderId).FirstOrDefault(),
                    GradedAt = s.Student.StudentGrades.Select(x => x.GradedAt).FirstOrDefault()
                })
                .FirstOrDefaultAsync(cancellationToken);

            return submission;
        }
        public async Task CreateAssignmentSubmissionAsync(AssignmentSubmission assignmentSubmission, CancellationToken cancellationToken)
        {
            await _dbContext.AssignmentSubmissions.AddAsync(assignmentSubmission, cancellationToken);
        }
        public async Task<bool> IsAssignmentSubmittedAsync(int studentId, int itemId, CancellationToken cancellationToken)
        {
            return await _dbContext.AssignmentSubmissions
                .AnyAsync(s => s.AssignmentId == itemId && s.StudentId == studentId, cancellationToken);
        }
        public async Task<List<AssignmentDeadlineReminderDto>> GetAssignmentsEndingWithin24HoursAsync(CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var in24Hours = now.AddHours(24);

            return await _dbContext.Assignments
                .AsNoTracking()
                .Where(a => a.DueDate > now && a.DueDate <= in24Hours && a.ModuleItem.Status == CourseStatus.Published)
                .SelectMany(a => a.Course.Enrollments, (a, e) => new { Assignment = a, Enrollment = e })
                .Where(x => !x.Assignment.Submissions.Any(s => s.StudentId == x.Enrollment.StudentId))
                .Select(x => new AssignmentDeadlineReminderDto
                {
                    StudentEmail = x.Enrollment.Student.User.Email!,
                    StudentName = $"{x.Enrollment.Student.User.FirstName} {x.Enrollment.Student.User.LastName}",
                    AssignmentTitle = x.Assignment.ModuleItem.Title,
                    CourseTitle = x.Assignment.Course.Title,
                    DueDate = x.Assignment.DueDate,
                    CourseId = x.Assignment.CourseId
                }).ToListAsync(cancellationToken);
        }
    }
}