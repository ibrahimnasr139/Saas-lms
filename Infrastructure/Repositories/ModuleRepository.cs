using Application.Features.Modules.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Enums;

namespace Infrastructure.Repositories
{
    internal sealed class ModuleRepository : IModuleRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public ModuleRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<int> CreateModule(Module module, CancellationToken cancellationToken)
        {
            await _context.Modules.AddAsync(module, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return module.Id;
        }
        public async Task DecreaseOrder(int moduleId, int courseId, int minOrder, CancellationToken cancellationToken, int maxOrder = int.MinValue)
        {
            await _context.Modules.Where(m => m.CourseId == courseId && m.Order > minOrder && m.Order <= maxOrder && m.Id != moduleId)
           .ExecuteUpdateAsync(m => m.SetProperty(p => p.Order, p => p.Order - 1), cancellationToken);
        }
        public async Task<List<AllModulesDto>> GetAllModulesAsync(int courseId, CancellationToken cancellationToken)
        {
            return await _context.Modules.Where(m => m.CourseId == courseId)
                .ProjectTo<AllModulesDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
        public async Task<int> GetMaxOrder(int courseId, CancellationToken cancellationToken)
        {
            var maxOrder = await _context.Modules
                .Where(m => m.CourseId == courseId)
                .MaxAsync(m => (int?)m.Order, cancellationToken);
            return maxOrder ?? 0;
        }
        public async Task<Module?> GetModuleByIdAsync(int moduleId, int courseId, string subdomain, CancellationToken cancellationToken)
        {
            return await _context.Modules.FirstOrDefaultAsync(c => c.Id == moduleId && c.CourseId == courseId && c.Course.Tenant.SubDomain == subdomain, cancellationToken);
        }
        public async Task<ModuleDto?> GetModuleWithItemsAsync(int moduleId, int courseId, string subdomain, CancellationToken cancellationToken)
        {
            return await _context.Modules.Where(m => m.Id == moduleId && m.CourseId == courseId && m.Course.Tenant.SubDomain == subdomain)
                .Select(a => new ModuleDto
                {
                    Title = a.Title,
                    Description = a.Description,
                    Order = a.Order,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt,
                    TotalItems = a.ModuleItems.Count(),
                    TotalLessons = a.Lessons.Count(),
                    TotalAssignments = a.Assignments.Count(),
                    TotalQuizzes = a.Quizzes.Count(),
                    Items = a.ModuleItems
                    .OrderBy(i => i.Order)
                    .Select(i => new ModuleItemDto
                    {
                        Id = i.Id,
                        ItemType = i.Type,
                        Status = i.Status,
                        Order = i.Order,
                        MetaData = new ItemMetaDataDto
                        {
                            Title = i.Title,
                            Description = i.Description,
                            CreatedAt = i.CreatedAt,
                            UpdatedAt = i.UpdatedAt,
                            Views = i.Type == ModuleItemType.Lesson ? i.Lesson!.LessonViews.Count() : null,
                            QuestionsCount = i.Type == ModuleItemType.Quiz ? i.Quiz!.Questions.Count() : null,
                            PassingScore = i.Type == ModuleItemType.Quiz ? i.Quiz!.PassingScore : null,
                            AverageScore = i.Type == ModuleItemType.Quiz ? i.Quiz!.Attempts.Average(s => s.Score) : null,
                            Attempts = i.Type == ModuleItemType.Quiz ? i.Quiz!.Attempts.Count() : null,
                            Submissions = i.Type == ModuleItemType.Assignment ? i.Assignment!.Submissions.Count() : null
                        }
                    }).ToList()
                }).FirstOrDefaultAsync(cancellationToken);
        }
        public async Task IncreaseOrder(int moduleId, int courseId, int minOrder, CancellationToken cancellationToken, int maxOrder = int.MaxValue)
        {
            await _context.Modules.Where(m => m.CourseId == courseId && m.Order >= minOrder && m.Order < maxOrder && m.Id != moduleId)
           .ExecuteUpdateAsync(m => m.SetProperty(p => p.Order, p => p.Order + 1), cancellationToken);
        }
        public async Task RemoveModule(Module module, CancellationToken cancellationToken)
        {
            _context.Modules.Remove(module);
            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task<int?> GetFirstModuleIdAsync(int courseId, CancellationToken cancellationToken)
        {
            return await _context.Modules
                .Where(m => m.CourseId == courseId && m.Status == CourseStatus.Published)
                .Select(m => (int?)m.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<string?> GetModuleNameAsync(int itemId, int courseId, CancellationToken cancellationToken)
        {
            var result = await _context.ModuleItems
                .Where(mi => mi.Id == itemId && mi.CourseId == courseId)
                .Select(mi => mi.Module!.Title)
                .FirstOrDefaultAsync(cancellationToken);
            return result;
        }
        public Task<string> GetModuleTitleAsync(int moduleId, string subDomain, CancellationToken cancellationToken)
        {
            var title = _context.Modules
                .Where(m => m.Id == moduleId && m.Course.Tenant.SubDomain == subDomain)
                .Select(m => m.Title)
                .FirstOrDefaultAsync(cancellationToken);
            return title!;
        }
    }
}