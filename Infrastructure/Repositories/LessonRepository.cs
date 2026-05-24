using Application.Features.Lessons.Dtos;
using Application.Features.StudentLessons.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Enums;

namespace Infrastructure.Repositories
{
    internal sealed class LessonRepository : ILessonRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public LessonRepository(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<List<StudentViewsDto>> GetAllStudentsViewsAsync(int courseId, int itemId, CancellationToken cancellationToken)
        {
            return await _dbContext.Students.Where(s => s.Enrollments.Any(c => c.CourseId == courseId))
                .LeftJoin(_dbContext.LessonViews.Where(sv => sv.ModuleItemId == itemId),
                    student => student.Id,
                    studentView => studentView.StudentId,
                    (student, studentView) => new StudentViewsDto
                    {
                        Id = studentView != null ? studentView.Id : 0,
                        StudentId = student.Id,
                        StudentName = student.User.FirstName + " " + student.User.LastName,
                        ProfilePicture = student.User.ProfilePicture,
                        Status = studentView != null ? studentView.Status : ViewStatus.NotStarted,
                        LatestProgress = studentView != null ? studentView.LastPositionSeconds : null,
                        TotalWatchTime = studentView != null ? studentView.WatchedSeconds : 0,
                        LastViewTime = studentView != null ? studentView.LastWatchedAt : null,
                        Device = studentView != null ? studentView.Device : null
                    }).ToListAsync(cancellationToken);
        }
        public async Task<LessonOverviewDto?> GetLessonOverviewAsync(int itemId, CancellationToken cancellationToken)
        {
            return await _dbContext.LessonViews.Where(lv => lv.ModuleItemId == itemId)
                .GroupBy(lv => lv.ModuleItemId)
                .Select(g => new LessonOverviewDto
                {
                    TotalViews = g.Sum(s => s.ViewCount),
                    CompletionRate = g.Count(c => c.Status == ViewStatus.Completed) * 1.0 / g.Count(),
                    AverageWatchTime = g.Average(s => s.WatchedSeconds),
                    TotalStudents = g.Select(s => s.StudentId).Count()
                }).FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<DateTime> GetPeakActivityTimeAsync(int itemId, CancellationToken cancellationToken)
        {
            return await _dbContext.LessonViews.Where(lv => lv.ModuleItemId == itemId)
                 .GroupBy(lv => lv.CreatedAt)
                 .Select(g => new
                 {
                     TimeSlot = g.Key,
                     ViewCount = g.Sum(s => s.ViewCount)
                 })
                 .OrderByDescending(g => g.ViewCount)
                 .Select(g => (DateTime)g.TimeSlot)
                 .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<List<ViewsOverTime>> GetViewsOverTimeAsync(int itemId, CancellationToken cancellationToken)
        {
            return await _dbContext.LessonViews.Where(lv => lv.ModuleItemId == itemId && lv.CreatedAt >= DateTime.UtcNow.AddDays(-7))
                .GroupBy(lv => lv.CreatedAt.Date)
                .Select(g => new ViewsOverTime
                {
                    Date = g.Key,
                    TotalViews = g.Sum(s => s.ViewCount)
                }).ToListAsync(cancellationToken);
        }
        public async Task<bool> IsFound(int id, int moduleId, int courseId, string subdomain, CancellationToken cancellationToken)
        {
            return await _dbContext.Lessons.AnyAsync(l => l.ModuleItemId == id && l.ModuleId == moduleId && l.CourseId == courseId && l.Course.Tenant.SubDomain == subdomain, cancellationToken);
        }
        public async Task<StudentLessonProgressDto> GetStudentLessonProgressAsync(int studentId, int courseId, int itemId, CancellationToken cancellationToken)
        {
            var result = await _dbContext.LessonViews
                .Where(lv => lv.StudentId == studentId && lv.ModuleItemId == itemId && lv.Lesson.CourseId == courseId)
                .ProjectTo<StudentLessonProgressDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);
            return result!;
        }
        public async Task<string> GetVideoIdAsync(int itemId, int courseId, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Lessons.Where(l => l.ModuleItemId == itemId && l.CourseId == courseId)
                .Select(l => l.VideoId)
                .FirstOrDefaultAsync(cancellationToken);
            return result!;
        }
        public async Task<StudentLessonTranscriptDto?> GetStudentLessonTranscriptAsync(string videoId, CancellationToken cancellationToken)
        {
            var timestamps = await _dbContext.VideoTimestamps
                .Where(vt => vt.FileId == videoId)
                .ToListAsync(cancellationToken);

            if (!timestamps.Any())
                return null;

            return new StudentLessonTranscriptDto
            {
                TotalSegments = timestamps.Count,
                Transcript = timestamps.Select(vt => new TranscriptDto
                {
                    Id = vt.Id,
                    Text = vt.Text,
                    Start = vt.StartTime,
                    End = vt.EndTime
                }).ToList()
            };
        }
        public async Task<string> GetLessonNameAsync(int itemId, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Lessons.Where(l => l.ModuleItemId == itemId)
                .Select(l => l.ModuleItem.Title)
                .FirstOrDefaultAsync(cancellationToken);
            return result!;
        }
        public async Task CreateAiAssistantMessageAsync(List<AiAssistantMessage> messages, CancellationToken cancellationToken)
        {
            await _dbContext.AiAssistantMessages.AddRangeAsync(messages, cancellationToken);
        }
        public async Task<List<AiChatMessage>?> GetAiChatMessagesAsync(int itemId, int studentId, CancellationToken cancellationToken)
        {
            return await _dbContext.AiAssistantMessages
                .Where(ai => ai.LessonId == itemId && ai.StudentId == studentId)
                .Select(ai => new AiChatMessage
                {
                    Id = ai.Id,
                    Content = ai.Content,
                    Role = ai.Role,
                    CreatedAt = ai.CreatedAt
                }).ToListAsync(cancellationToken);
        }
        public async Task<LessonContentDto?> GetLessonContentAsync(int courseId, int moduleId, int itemId, CancellationToken cancellationToken)
        {
            var lesson = await _dbContext.Lessons
                .Where(l => l.ModuleItemId == itemId && l.ModuleId == moduleId && l.CourseId == courseId)
                .Select(l => new
                {
                    VideoId = l.VideoId,
                    VideoUrl = l.File.Url,
                    Duration = l.File.Metadata != null && l.File.Metadata.ContainsKey("Duration")
                        ? int.Parse(l.File.Metadata["Duration"])
                        : 0,
                    Resources = l.Resources,
                    CreatedAt = l.ModuleItem.CreatedAt,
                    UpdatedAt = l.ModuleItem.UpdatedAt,
                }).FirstOrDefaultAsync(cancellationToken);

            if (lesson is null)
                return null;

            return new LessonContentDto
            {
                Type = ModuleItemType.Lesson.ToString(),
                VideoId = lesson.VideoId,
                VideoUrl = lesson.VideoUrl,
                Duration = lesson.Duration,
                Resources = lesson.Resources.Select(r => new ResourceDto
                {
                    Name = r.Name,
                    Url = r.Url
                }).ToList(),
                CreatedAt = lesson.CreatedAt,
                UpdatedAt = lesson.UpdatedAt,
            };
        }
    }
}