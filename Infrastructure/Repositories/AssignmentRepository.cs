using Application.Features.Assignments.Dtos;
using Application.Features.StudentAssignments.Dtos;
using AutoMapper;
using Domain.Enums;

namespace Infrastructure.Repositories
{
    internal sealed class AssignmentRepository : IAssignmentRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public AssignmentRepository(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<OverviewDto?> GetOverviewAsync(int itemId, CancellationToken cancellationToken)
        {
            return await _dbContext.AssignmentSubmissions.Where(s => s.AssignmentId == itemId)
                .GroupBy(s => 1)
                .Select(g => new OverviewDto
                {
                    TotalSubmissions = g.Count(),
                    AverageScore = g.Average(s => s.EarnedMarks),
                    HeighestScore = g.Max(s => s.EarnedMarks),
                    LowestScore = g.Min(s => s.EarnedMarks)
                }).FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<List<StudentSubmissionDto>> GetSubmissionsAsync(int courseId, int itemId, CancellationToken cancellationToken)
        {
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
                         TotalMarks = studentSubmission != null ? studentSubmission.Assignment.Marks : 0,
                         File = studentSubmission != null ? new FileDto
                         {
                             FileName = studentSubmission.File!.Name,
                             Url = studentSubmission.File.Url
                         } : null
                     }
                 ).ToListAsync(cancellationToken);
        }
        public async Task<StudentAssignmentDto> GetStudentAssignmentAsync(int studentId, int itemId, int courseId, CancellationToken cancellationToken)
        {
            var result = await _dbContext.ModuleItems
                .Where(dt => dt.Id == itemId && dt.CourseId == courseId)
                .Include(mi => mi.Assignment)
                    .ThenInclude(a => a!.Submissions.Where(s => s.StudentId == studentId))
                        .ThenInclude(s => s.Student)
                            .ThenInclude(sg => sg.StudentGrades)
                .Include(a => a.Assignment)
                    .ThenInclude(a => a!.Submissions.Where(s => s.StudentId == studentId))
                        .ThenInclude(s => s.File)
                .FirstOrDefaultAsync(cancellationToken);
            return _mapper.Map<StudentAssignmentDto>(result!);
        }
        public async Task CreateAssignmentSubmissionAsync(AssignmentSubmission assignmentSubmission, CancellationToken cancellationToken)
        {
            await _dbContext.AssignmentSubmissions.AddAsync(assignmentSubmission, cancellationToken);
        }
    }
}