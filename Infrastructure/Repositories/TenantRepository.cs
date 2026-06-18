using Application.Constants;
using Application.Features.Tenants.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Constants;
using Domain.Enums;

namespace Infrastructure.Repositories
{
    internal sealed class TenantRepository : ITenantRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        public TenantRepository(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task AddTenantMemberAsync(TenantMember tenantMember, CancellationToken cancellationToken)
        {
            await _dbContext.TenantMembers.AddAsync(tenantMember, cancellationToken);
        }
        public async Task<(int ownerRoleId, int assistantRoleId)> AddTenantRoles(int tenantId, CancellationToken cancellationToken)
        {
            var roles = new List<TenantRole>
            {
                new TenantRole { Name = RolesConstants.Owner, TenantId = tenantId, HasAllPermissions = true },
                new TenantRole { Name = RolesConstants.Assistant, TenantId = tenantId }
            };
            var ownerRole = await _dbContext.TenantRoles.AddAsync(roles[0], cancellationToken);
            var assistantRole = await _dbContext.TenantRoles.AddAsync(roles[1], cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return (ownerRole.Entity.Id, assistantRole.Entity.Id);
        }
        public async Task<int> CreateTenantAsync(Tenant tenant, CancellationToken cancellationToken)
        {
            await _dbContext.Tenants.AddAsync(tenant, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return tenant.Id;
        }
        public async Task<bool> IsSubDomainExistsAsync(string subDomain, CancellationToken cancellationToken)
        {
            return await _dbContext.Tenants
                .AnyAsync(t => t.SubDomain == subDomain, cancellationToken);
        }
        public async Task<LastTenantDto?> GetLastTenantAsync(string? subDomain, CancellationToken cancellationToken)
        {
            return await _dbContext.Tenants
                .AsNoTracking()
                .ProjectTo<LastTenantDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(t => t.SubDomain == subDomain, cancellationToken);
        }
        public async Task AssignAssistantPermissions(int assistantRoleId, CancellationToken cancellationToken)
        {
            var permissions = GetAssistantPermissions().Select(permission => new Permission
            {
                Id = permission
            }).ToList();
            _dbContext.AttachRange(permissions);
            var rolePermissions = permissions.Select(permission => new RolePermission
            {
                TenantRoleId = assistantRoleId,
                PermissionId = permission.Id
            });
            await _dbContext.RolePermissions.AddRangeAsync(rolePermissions, cancellationToken);
        }
        public Task<int> GetTenantIdAsync(string subDomain, CancellationToken cancellationToken)
        {
            return _dbContext.Tenants
                .AsNoTracking()
                .Where(t => t.SubDomain == subDomain)
                .Select(t => t.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<TenantUsageDto> GetTenantUsageAsync(string subDomain, CancellationToken cancellationToken)
        {
            var result = await (from s in _dbContext.Subscriptions.AsNoTracking()
                                join tu in _dbContext.TenantUsage on s.Id equals tu.SubscriptionId
                                join pf in _dbContext.PlanFeatures on tu.PlanFeatureId equals pf.Id
                                join p in _dbContext.Plans on pf.PlanId equals p.Id
                                join f in _dbContext.Features on pf.FeatureId equals f.Id
                                where s.Tenant.SubDomain == subDomain
                                group new { tu, pf, f } by new
                                {
                                    s.Id,
                                    s.Status,
                                    s.StartsAt,
                                    s.EndsAt,
                                    PlanId = p.Id,
                                    PlanName = p.Name
                                } into g
                                select new TenantUsageDto
                                {
                                    Subscription = new SubscriptionDto
                                    {
                                        Id = g.Key.Id,
                                        Status = g.Key.Status,
                                        Start = g.Key.StartsAt,
                                        End = g.Key.EndsAt,
                                        Plan = new SubscriptionPlanDto
                                        {
                                            Id = g.Key.PlanId,
                                            Name = g.Key.PlanName
                                        }
                                    },
                                    Usage = g.Select(x => new UsageDto
                                    {
                                        FeatureKey = x.f.Key,
                                        Name = x.f.Name,
                                        Used = x.tu.Used,
                                        Limit = x.pf.LimitValue,
                                        Unit = x.pf.LimitUnit,
                                        Enabled = x.pf.IsEnabled
                                    }).ToList()
                                }
                    ).FirstOrDefaultAsync(cancellationToken);

            return result!;
        }
        public async Task InitializeTenantUsageAsync(List<Guid> PlanFeatureId, int SubscriptionId, int TenantId)
        {
            var tenantUsages = PlanFeatureId.Select(planFeatureId => new TenantUsage
            {
                PlanFeatureId = planFeatureId,
                SubscriptionId = SubscriptionId,
                TenantId = TenantId
            }).ToList();

            _dbContext.TenantUsage.AddRange(tenantUsages);
        }
        public async Task<ContentLibraryResourceDto> GetTenantLibraryResource(string subDomain, FileType Type, string? Q, CancellationToken cancellationToken)
        {
            var query = _dbContext.Files
                .AsNoTracking()
                .Where(f => f.Tenant!.SubDomain == subDomain && f.Type == Type);

            if (!string.IsNullOrWhiteSpace(Q))
                query = query.Where(f => f.Name.Contains(Q));

            var files = await query.ToListAsync(cancellationToken);
            return new ContentLibraryResourceDto
            {
                Resources = new ResourceDto
                {
                    Documents = Type == FileType.Document ? _mapper.Map<List<DocumentDto>>(files) : new(),
                    Videos = Type == FileType.Video ? _mapper.Map<List<VideoDto>>(files) : new(),
                    Images = Type == FileType.Image ? _mapper.Map<List<ImageDto>>(files) : new(),
                }
            };
        }
        public async Task<ContentLibraryStatisticsDto> GetStatisticsAsync(int TenantId, CancellationToken cancellationToken)
        {
            var files = await _dbContext.Files
                .Where(f => f.TenantId == TenantId)
                .ToListAsync(cancellationToken);

            return new ContentLibraryStatisticsDto
            {
                TotalFiles = files.Count,
                TotalDocuments = files.Count(f => f.Type == FileType.Document),
                TotalVideos = files.Count(f => f.Type == FileType.Video),
                TotalImages = files.Count(f => f.Type == FileType.Image),
            };
        }
        public async Task<bool> IsFeatureUsingEnded(string subDomain, string featureName, CancellationToken cancellationToken)
        {
            var isEnded = await _dbContext.TenantUsage
                .AsNoTracking()
                .Where(tu => tu.Tenant.SubDomain == subDomain && (tu.Subscription.Status == SubscriptionStatus.Active || tu.Subscription.Status == SubscriptionStatus.Trialed) &&
                 tu.Subscription.EndsAt > DateTime.UtcNow && tu.PlanFeature.Feature.Key == featureName)
                .Select(tu => new { tu.Used, tu.PlanFeature.LimitValue })
                .FirstOrDefaultAsync(cancellationToken);
            return isEnded != null && isEnded.Used >= isEnded.LimitValue;
        }
        public Task<string> GetSubDomainAsync(int tenantId, CancellationToken cancellationToken)
        {
            var SubDomain = _dbContext.Tenants
                .AsNoTracking()
                .Where(t => t.Id == tenantId)
                .Select(t => t.SubDomain)
                .FirstOrDefaultAsync(cancellationToken);
            return SubDomain!;
        }
        public async Task IncreasePlanFeatureUsageByKeyAsync(string subDomain, string featureName, CancellationToken cancellationToken, long Size = 1)
        {
            await _dbContext.TenantUsage
                .Where(tu => tu.Tenant.SubDomain == subDomain && tu.PlanFeature.Feature.Key == featureName)
                .ExecuteUpdateAsync(s => s.SetProperty(tu => tu.Used, tu => tu.Used + Size), cancellationToken);
        }
        public async Task DecreasePlanFeatureUsageByKeyAsync(string subDomain, string featureName, CancellationToken cancellationToken, long Size = 1)
        {
            await _dbContext.TenantUsage
                .Where(tu => tu.Tenant.SubDomain == subDomain && tu.PlanFeature.Feature.Key == featureName)
                .ExecuteUpdateAsync(s => s.SetProperty(tu => tu.Used, tu => tu.Used - Size), cancellationToken);
        }
        private List<string> GetAssistantPermissions()
        {
            return new List<string>
            {
                PermissionConstants.CREATE_ASSIGNMENTS,
                PermissionConstants.VIEW_ASSIGNMENTS,
                PermissionConstants.MANAGE_ASSIGNMENTS,
                PermissionConstants.GRADE_ASSIGNMENTS,
                PermissionConstants.VIEW_COURSES,
                PermissionConstants.EDIT_COURSES,
                PermissionConstants.MANAGE_LESSONS,
                PermissionConstants.MANAGE_MEMBERS,
                PermissionConstants.MANAGE_VIDEOS,
                PermissionConstants.MANAGE_MODULE_ITEMS,
                PermissionConstants.VIEW_MEMBERS,
                PermissionConstants.VIEW_MEMBER_PROFILE,
                PermissionConstants.VIEW_DASHBOARD,
                PermissionConstants.VIEW_ANALYTICS,
                PermissionConstants.VIEW_PERFORMANCE_CHART,
                PermissionConstants.VIEW_RECORDINGS,
                PermissionConstants.INVITE_STUDENTS,
                PermissionConstants.MANAGE_QUIZZES,
                PermissionConstants.CREATE_QUIZZES,
                PermissionConstants.VIEW_QUIZZES,
                PermissionConstants.VIEW_QUESTION_BANK,
                PermissionConstants.GRADE_QUIZZES,
            };
        }
    }
}