using Application.Features.StudentCourse.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Enums;

namespace Infrastructure.Repositories
{
    internal sealed class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public EnrollmentRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task CreateEnrollmentAsync(Enrollment enrollment, CancellationToken cancellationToken)
        {
            await _context.Enrollments.AddAsync(enrollment);
        }
        public async Task<bool> StudentIsAlreadyEnrolledAsync(int studentId, int courseId, CancellationToken cancellationToken)
        {
            return await _context.Enrollments
                 .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId, cancellationToken);
        }
        public async Task<List<string>> GetEmailsByCourseIdsAsync(int[] courseIds, CancellationToken cancellationToken)
        {
            return await _context.Enrollments
                .Where(e => courseIds.Contains(e.CourseId))
                .Select(e => e.Student.User.Email!)
                .Distinct()
                .ToListAsync(cancellationToken);
        }
        public async Task<List<string>> GetAllStudentEmailsAsync(int tenantId, CancellationToken cancellationToken)
        {
            return await _context.Enrollments
                .Where(e => e.Course.TenantId == tenantId)
                .Select(e => e.Student.User.Email!)
                .Distinct()
                .ToListAsync(cancellationToken);
        }
        public async Task<List<StudentCoursesDto>> GetStudentCoursesAsync(int studentId, CancellationToken cancellationToken)
        {
            return await _context.Enrollments
                .Where(e => e.StudentId == studentId)
                .ProjectTo<StudentCoursesDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
        public async Task<StudentCourseDto?> GetStudentCourseAsync(int studentId, int courseId, CancellationToken cancellationToken)
        {
            return await _context.Enrollments
                .AsNoTracking()
                .Where(e => e.StudentId == studentId && e.CourseId == courseId)
                .ProjectTo<StudentCourseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<List<StudentModuleDto>> GetStudentCourseModulesAsync(int studentId, int courseId, CancellationToken cancellationToken)
        {
            var enrollment = await _context.Enrollments
                .AsNoTracking()
                .Where(e => e.StudentId == studentId && e.CourseId == courseId)
                .Select(e => new
                {
                    e.CurrentModuleItemId,
                    Modules = e.Course.Modules
                        .Where(m => m.Status == CourseStatus.Published)
                        .OrderBy(mi => mi.Order)
                        .Select(m => new
                        {
                            m.Id,
                            m.Title,
                            m.Description,
                            TotalItems = m.ModuleItems.Count(mi => mi.Status == CourseStatus.Published),
                            IsCurrentModule = m.ModuleItems.Any(mi => mi.Id == e.CurrentModuleItemId),
                            ModuleItems = m.ModuleItems
                                .Where(mi => mi.Status == CourseStatus.Published)
                                .OrderBy(mi => mi.Order)
                                .Select(mi => new
                                {
                                    mi.Id,
                                    mi.Title,
                                    mi.Type,
                                    DueDate = mi.Assignment != null ? (DateTime?)mi.Assignment.DueDate : null,
                                    IsCompleted =
                                        _context.LessonViews.Any(lv => lv.ModuleItemId == mi.Id && lv.StudentId == studentId && lv.Status == ViewStatus.Completed) ||
                                        _context.QuizAttempts.Any(qa => qa.ModuleItemId == mi.Id && qa.StudentId == studentId) ||
                                        _context.AssignmentSubmissions.Any(asub => asub.AssignmentId == mi.Id && asub.StudentId == studentId),
                                    Conditions = mi.Conditions
                                        .Where(c => c.Enabled)
                                        .Select(c => new
                                        {
                                            c.ConditionType,
                                            c.Value,
                                            c.RequiredModuleItemId,
                                            c.Message,
                                            IsMet =
                                                c.ConditionType == ConditionType.completed &&
                                                    _context.LessonViews.Any(lv => lv.ModuleItemId == c.RequiredModuleItemId && lv.StudentId == studentId && lv.Status == ViewStatus.Completed) ||
                                                c.ConditionType == ConditionType.passed &&
                                                    _context.QuizAttempts.Any(qa => qa.ModuleItemId == c.RequiredModuleItemId && qa.StudentId == studentId && qa.Score >= qa.Quiz.PassingScore) ||
                                                c.ConditionType == ConditionType.score_gte &&
                                                    _context.QuizAttempts.Any(qa => qa.ModuleItemId == c.RequiredModuleItemId && qa.StudentId == studentId && qa.Score >= c.Value) ||
                                                c.ConditionType == ConditionType.submitted &&
                                                    _context.AssignmentSubmissions.Any(asub => asub.AssignmentId == c.RequiredModuleItemId && asub.StudentId == studentId)
                                        }).ToList()
                                }).ToList()
                        }).ToList()
                }).FirstOrDefaultAsync(cancellationToken);

            if (enrollment is null)
                return [];

            return enrollment.Modules.Select(m => new StudentModuleDto
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                TotalItems = m.TotalItems,
                CompletedItems = m.ModuleItems.Count(mi => mi.IsCompleted),
                IsCurrentModule = m.IsCurrentModule,
                ModuleItems = m.ModuleItems.Select(mi =>
                {
                    ModuleItemStatus status;

                    if (mi.IsCompleted)
                        status = ModuleItemStatus.completed;
                    else
                    {
                        if (mi.Conditions.Any())
                        {
                            var allConditionsMet = mi.Conditions.All(c => c.IsMet);
                            status = allConditionsMet ? ModuleItemStatus.avilable : ModuleItemStatus.locked;
                        }
                        else
                            status = ModuleItemStatus.avilable;
                    }

                    return new ModuleItemDto
                    {
                        Id = mi.Id,
                        Title = mi.Title,
                        Type = mi.Type,
                        Status = status,
                        DueDate = mi.DueDate,
                        Reason = mi.Conditions
                            .Where(c => !c.IsMet)
                            .Select(c => c.Message)
                            .FirstOrDefault()
                    };
                }).ToList()
            }).ToList();
        }
        public async Task<int> GetTenantIdAsync(int studentId, int courseId, CancellationToken cancellationToken)
        {
            return await _context.Enrollments
                .Where(e => e.StudentId == studentId && e.CourseId == courseId)
                .Select(e => e.TenantId)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}