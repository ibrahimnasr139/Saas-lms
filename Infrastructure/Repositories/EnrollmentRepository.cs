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
            //var currentModuleItemId = await _context.Enrollments
            //    .AsNoTracking()
            //    .Where(e => e.StudentId == studentId && e.CourseId == courseId 
            //        && e.Module!.Status == CourseStatus.Published && e.ModuleItem!.Status == CourseStatus.Published)
            //    .Select(e => e.CurrentModuleItemId)
            //    .FirstOrDefaultAsync(cancellationToken);

            //if (currentModuleItemId is null)
            //    return null;

            //return await _context.Enrollments
            //    .AsNoTracking()
            //    .Where(e => e.StudentId == studentId && e.CourseId == courseId)
            //    .SelectMany(e => e.Course.Modules)
            //    .Select(m => new StudentModuleDto
            //    {
            //        Id = m.Id,
            //        Title = m.Title,
            //        Description = m.Description,
            //        TotalItems = m.ModuleItems.Count(mi => mi.Status == CourseStatus.Published),
            //        CompletedItems = 0,
            //        IsCurrentModule = m.ModuleItems.Any(mi => mi.Id == currentModuleItemId),
            //        ModuleItems = m.ModuleItems.Select(mi => new ModuleItemDto
            //        {
            //            Id = mi.Id,
            //            Title = mi.Title,
            //            Type = mi.Type,
            //            DueDate = mi.Assignment != null ? mi.Assignment.DueDate : null
            //        }).ToList()
            //    }).ToListAsync(cancellationToken);


            var enrollment = await _context.Enrollments
                .AsNoTracking()
                .Where(e => e.StudentId == studentId && e.CourseId == courseId)
                .Select(e => new
                {
                    e.CurrentModuleItemId,
                    Modules = e.Course.Modules
                        .Where(m => m.Status == CourseStatus.Published)
                        .Select(m => new StudentModuleDto
                        {
                            Id = m.Id,
                            Title = m.Title,
                            Description = m.Description,
                            TotalItems = m.ModuleItems.Count(mi => mi.Status == CourseStatus.Published),
                            CompletedItems = 0,
                            IsCurrentModule = m.ModuleItems.Any(mi => mi.Id == e.CurrentModuleItemId),
                            ModuleItems = m.ModuleItems
                                .Where(mi => mi.Status == CourseStatus.Published)
                                .Select(mi => new ModuleItemDto
                                {
                                    Id = mi.Id,
                                    Title = mi.Title,
                                    Type = mi.Type,
                                    DueDate = mi.Assignment != null ? mi.Assignment.DueDate : null
                                }).ToList()
                        }).ToList()
                }).FirstOrDefaultAsync(cancellationToken);

            if (enrollment is null)
                return [];

            return enrollment.Modules;
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