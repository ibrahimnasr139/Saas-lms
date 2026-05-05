using Application.Features.ModuleItems.Commands.ReorderModuleItem;
using Application.Features.ModuleItems.Dtos;
using Application.Features.StudentLessons.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Enums;

namespace Infrastructure.Repositories
{
    internal sealed class ModuleItemRepository : IModuleItemRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        public ModuleItemRepository(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task AddTenantQuestionsAsync(int quizId, IEnumerable<QuizQuestion> questions, CancellationToken cancellationToken)
        {
            await _dbContext.QuizQuestions.Where(q => q.QuizId == quizId).ExecuteDeleteAsync(cancellationToken);
            await _dbContext.QuizQuestions.AddRangeAsync(questions, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task CreateAssignment(Assignment assignment, CancellationToken cancellationToken)
        {
            await _dbContext.Assignments.AddAsync(assignment);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task CreateLesson(Lesson lesson, CancellationToken cancellationToken)
        {
            await _dbContext.Lessons.AddAsync(lesson);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task<int> CreateModuleItem(ModuleItem moduleItem, CancellationToken cancellationToken)
        {
            await _dbContext.ModuleItems.AddAsync(moduleItem);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return moduleItem.Id;
        }
        public async Task CreateQuiz(Quiz quiz, CancellationToken cancellationToken)
        {
            await _dbContext.Quizzes.AddAsync(quiz);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task<List<AllItemsDto>> GetAllItemsAsync(int moduleItemId, int moduleId, ModuleItemType? type, int courseId, CancellationToken cancellationToken)
        {
            var currentModuleItem = await _dbContext.ModuleItems
                .AsNoTracking()
                .Where(mi => mi.Id == moduleItemId && mi.ModuleId == moduleId && mi.CourseId == courseId)
                .FirstOrDefaultAsync(cancellationToken);

            var query = _dbContext.ModuleItems
                .AsNoTracking()
                .Where(mi => mi.ModuleId == moduleId && mi.CourseId == courseId && mi.Status == CourseStatus.Published
                    && mi.Order < currentModuleItem!.Order);

            if (type.HasValue)
                query = query.Where(mi => mi.Type == type.Value);

            return await query
                .ProjectTo<AllItemsDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
        public async Task<AssignmentDto?> GetAssignmentAsync(int moduleItemId, int moduleId, int courseId, string subdomain, CancellationToken cancellationToken)
        {
            return await _dbContext.Assignments
                .AsNoTracking()
                .Where(tm => tm.ModuleItemId == moduleItemId && tm.ModuleId == moduleId && tm.CourseId == courseId && tm.Course.Tenant.SubDomain == subdomain)
                .ProjectTo<AssignmentDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<Assignment?> GetAssignmentByModuleItemIdAsync(int moduleItemId, int moduleId, int courseId, string subdomain, CancellationToken cancellationToken)
        {
            return await _dbContext.Assignments.
                 Include(x => x.ModuleItem).
                 FirstOrDefaultAsync(l => l.ModuleItemId == moduleItemId && l.ModuleId == moduleId && l.CourseId == courseId && l.Course.Tenant.SubDomain == subdomain, cancellationToken);
        }
        public async Task<ModuleItem?> GetAsync(int moduleItemId, int moduleId, int courseId, string subdomain, CancellationToken cancellationToken)
        {
            return await _dbContext.ModuleItems
                .FirstOrDefaultAsync(m => m.Id == moduleItemId && m.ModuleId == moduleId && m.CourseId == courseId && m.Course.Tenant.SubDomain == subdomain, cancellationToken);
        }
        public async Task<ModuleItem?> GetItemConditions(int moduleItemId, int moduleId, int courseId, string subdomain, CancellationToken cancellationToken)
        {
            return await _dbContext.ModuleItems
                .Include(mi => mi.Conditions)
                .FirstOrDefaultAsync(m => m.Id == moduleItemId && m.ModuleId == moduleId && m.CourseId == courseId && m.Course.Tenant.SubDomain == subdomain, cancellationToken);
        }
        public async Task<Lesson?> GetLessonByModuleItemIdAsync(int moduleItemId, int moduleId, int courseId, string subdomain, CancellationToken cancellationToken)
        {
            return await _dbContext.Lessons.
                 Include(x => x.ModuleItem).
                 FirstOrDefaultAsync(l => l.ModuleItemId == moduleItemId && l.ModuleId == moduleId && l.CourseId == courseId && l.Course.Tenant.SubDomain == subdomain, cancellationToken);
        }
        public async Task<Quiz?> GetQuizAsync(int moduleItemId, int moduleId, int courseId, string subdomain, CancellationToken cancellationToken)
        {
            return await _dbContext.Quizzes
                .FirstOrDefaultAsync(l => l.ModuleItemId == moduleItemId && l.ModuleId == moduleId && l.CourseId == courseId && l.Course.Tenant.SubDomain == subdomain, cancellationToken);
        }
        public async Task<QuizDto?> GetQuizWithQuestions(int moduleItemId, int moduleId, int courseId, string subdomain, CancellationToken cancellationToken)
        {
            return await _dbContext.Quizzes
                .AsNoTracking()
                .Where(q => q.ModuleItemId == moduleItemId && q.ModuleId == moduleId && q.CourseId == courseId && q.Course.Tenant.SubDomain == subdomain)
                .ProjectTo<QuizDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<SettingsDto?> GetSettingsAsync(int moduleItemId, int moduleId, int courseId, string subdomain, CancellationToken cancellationToken)
        {
            return await _dbContext.ModuleItems
                .AsNoTracking()
                .Where(tm => tm.Id == moduleItemId && tm.ModuleId == moduleId && tm.CourseId == courseId && tm.Course.Tenant.SubDomain == subdomain)
                .ProjectTo<SettingsDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task RemoveAsync(ModuleItem moduleItem, CancellationToken cancellationToken)
        {
            _dbContext.ModuleItems.Remove(moduleItem);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task ReorderItems(IEnumerable<OrderDto> orders, CancellationToken cancellationToken)
        {
            foreach (var order in orders)
            {
                await _dbContext.ModuleItems
                    .Where(mi => mi.Id == order.Id)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(mi => mi.Order, order.Order), cancellationToken);
            }
        }
        public async Task<int?> GetFirstModuleItemAsync(int? moduleId, CancellationToken cancellationToken)
        {
            if (moduleId is null)
                return null;

            return await _dbContext.ModuleItems
                .Where(mt => mt.ModuleId == moduleId && mt.Status == CourseStatus.Published)
                .Select(mt => (int?)mt.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<bool> ModuleItemIsExistAsync(int moduleItemId, int courseId, CancellationToken cancellationToken)
        {
            return await _dbContext.ModuleItems
                .AnyAsync(mi => mi.Id == moduleItemId && mi.CourseId == courseId, cancellationToken);
        }
        public async Task<StudentLessonItemDto> GetStudentLessonItemAsync(int moduleItemId, int courseId, CancellationToken cancellationToken)
        {
            var result = await _dbContext.ModuleItems
                .Where(mi => mi.Id == moduleItemId && mi.CourseId == courseId)
                .Include(mi => mi.Lesson)
                    .ThenInclude(l => l!.File)
                .Include(mi => mi.Lesson)
                    .ThenInclude(l => l!.LessonViews)
                .FirstOrDefaultAsync(cancellationToken);

            if (result is null)
                return null!;
            return _mapper.Map<StudentLessonItemDto>(result);
        }
        public async Task<int> GetModuleIdAsync(int itemId, int courseId, CancellationToken cancellationToken)
        {
            return await _dbContext.ModuleItems
                .Where(mi => mi.Id == itemId && mi.CourseId == courseId)
                .Select(mi => mi.ModuleId)
                .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<int> GetMaxOrder(int courseId, int moduleId, CancellationToken cancellationToken)
        {
            var maxOrder = await _dbContext.ModuleItems
                .Where(x => x.CourseId == courseId && x.ModuleId == moduleId)
                .MaxAsync(x => (int?)x.Order, cancellationToken);
            return maxOrder ?? 0;
        }
        public async Task<List<bool>> GetConditionsStatusAsync(int studentId, int itemId, CancellationToken cancellationToken)
        {
            return await _dbContext.ModuleItemConditions
                .Where(c => c.ModuleItemId == itemId && c.Enabled)
                .Select(c => c.ConditionType == ConditionType.completed &&
                       _dbContext.LessonViews.Any(lv => lv.ModuleItemId == c.RequiredModuleItemId &&
                            lv.StudentId == studentId && lv.Status == ViewStatus.Completed) ||

                    c.ConditionType == ConditionType.passed &&
                        _dbContext.QuizAttempts.Any(qa => qa.ModuleItemId == c.RequiredModuleItemId &&
                            qa.StudentId == studentId && qa.Score >= qa.Quiz.PassingScore) ||

                    c.ConditionType == ConditionType.score_gte &&
                        _dbContext.QuizAttempts.Any(qa => qa.ModuleItemId == c.RequiredModuleItemId &&
                            qa.StudentId == studentId && qa.Score >= c.Value) ||

                    c.ConditionType == ConditionType.submitted &&
                        _dbContext.AssignmentSubmissions.Any(asub => asub.AssignmentId == c.RequiredModuleItemId &&
                            asub.StudentId == studentId)
                )
                .ToListAsync(cancellationToken);
        }
    }
}