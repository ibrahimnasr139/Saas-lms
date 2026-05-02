using Application.Features.Tenants.Dtos;
using Domain.Enums;

namespace Application.Contracts.Repositories
{
    public interface ITenantRepository
    {
        Task<bool> IsSubDomainExistsAsync(string subDomain, CancellationToken cancellationToken);
        Task<int> CreateTenantAsync(Tenant tenant, CancellationToken cancellationToken);
        Task<(int ownerRoleId, int assistantRoleId)> AddTenantRoles(int tenantId, CancellationToken cancellationToken);
        Task AddTenantMemberAsync(TenantMember member, CancellationToken cancellationToken);
        Task AssignAssistantPermissions(int assistantRoleId, CancellationToken cancellationToken);
        Task<LastTenantDto?> GetLastTenantAsync(string? subDomain, CancellationToken cancellationToken);
        Task<int> GetTenantIdAsync(string subDomain, CancellationToken cancellationToken);
        Task<TenantUsageDto> GetTenantUsageAsync(string subDomain, CancellationToken cancellationToken);
        Task InitializeTenantUsageAsync(List<Guid> PlanFeatureId, int SubscriptionId, int TenantId);
        Task<ContentLibraryResourceDto> GetTenantLibraryResource(int TenantId, FileType Type, string? Q, CancellationToken cancellationToken);
        Task<ContentLibraryStatisticsDto> GetStatisticsAsync(int TenantId, CancellationToken cancellationToken);
        Task<int> GetPlanFeatureUsageAsync(Guid PlanFeatureId, CancellationToken cancellationToken);
        Task InCreasePlanFeatureUsageAsync(int tenantId, Guid PlanFeatureId, long Size, CancellationToken cancellationToken);
        Task IncreasePlanFeatureUsageByKeyAsync(string subDomain, string featureName, CancellationToken cancellationToken, long Size = 1);
        Task DecreasePlanFeatureUsageByKeyAsync(string subDomain, string featureName, CancellationToken cancellationToken, long Size = 1);
        Task DeCreasePlanFeatureUsageAsync(int tenantId, Guid PlanFeatureId, long Size, CancellationToken cancellationToken);
        Task<bool> IsFeatureUsingEnded(string subDomain, string featureName, CancellationToken cancellationToken);
        Task<string> GetSubDomainAsync(int tenantId, CancellationToken cancellationToken);
    }
}