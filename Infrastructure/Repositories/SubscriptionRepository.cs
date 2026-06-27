using Application.Features.Tenants.Dtos;
using Domain.Enums;

namespace Infrastructure.Repositories
{
    internal class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly AppDbContext _context;
        public SubscriptionRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<int> CreateFreeSubcscription(int TenantId, Guid PlanPricingId, CancellationToken cancellationToken)
        {
            var subscription = new Subscription
            {
                TenantId = TenantId,
                PlanPricingId = PlanPricingId,
                StartsAt = DateTime.UtcNow,
                EndsAt = DateTime.UtcNow.AddDays(30),
                Status = SubscriptionStatus.Trialed
            };
            await _context.Subscriptions.AddAsync(subscription, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return subscription.Id;
        }
        public async Task<bool> HasActiveSubscriptionByTenantDomain(string subdomain, CancellationToken cancellationToken)
        {
            return await (from s in _context.Subscriptions
                          join t in _context.Tenants on s.TenantId equals t.Id
                          where t.SubDomain == subdomain &&
                                  (s.Status == SubscriptionStatus.Active ||
                                  s.Status == SubscriptionStatus.Trialed)
                                  && s.EndsAt > DateTime.UtcNow
                          select s).AnyAsync(cancellationToken);
        }
        public Task<Guid> GetPlanPricingIdAsync(int tenantId, CancellationToken cancellationToken)
        {
            return _context.Subscriptions.Where(s => s.TenantId == tenantId &&
                                (s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.Trialed) &&
                                s.EndsAt > DateTime.UtcNow)
                    .Select(s => s.PlanPricingId)
                    .FirstOrDefaultAsync(cancellationToken);
        }
        public Task<bool> TenantHasFeatureAsync(int tenantId, string featureKey, CancellationToken cancellationToken)
        {
            return (from s in _context.Subscriptions
                    join pp in _context.PlanPricings on s.PlanPricingId equals pp.Id
                    join pf in _context.PlanFeatures on pp.PlanId equals pf.PlanId
                    join f in _context.Features on pf.FeatureId equals f.Id
                    where s.TenantId == tenantId
                      && (s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.Trialed)
                      && s.EndsAt > DateTime.UtcNow
                      && f.Key == featureKey
                    select s.Id).AnyAsync(cancellationToken);
        }
        public async Task<List<ExpiredSubscriptionDto>> GetExpiredSubscriptionsAsync(CancellationToken cancellationToken)
        {
            return await _context.Subscriptions
                .Where(s => (s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.Trialed)
                         && s.EndsAt <= DateTime.UtcNow)
                .Select(s => new ExpiredSubscriptionDto
                {
                    SubscriptionId = s.Id,
                    TenantEmail = s.Tenant.Owner.Email!,
                    TenantName = $"{s.Tenant.Owner.FirstName} {s.Tenant.Owner.LastName}",
                    SubDomain = s.Tenant.SubDomain
                }).ToListAsync(cancellationToken);
        }
        public async Task<List<ExpiringSubscriptionDto>> GetSubscriptionsExpiringSoonAsync(CancellationToken cancellationToken)
        {
            var in3Days = DateTime.UtcNow.AddDays(3);
            return await _context.Subscriptions
                .Where(s => (s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.Trialed)
                         && s.EndsAt > DateTime.UtcNow
                         && s.EndsAt <= in3Days)
                .Select(s => new ExpiringSubscriptionDto
                {
                    TenantEmail = s.Tenant.Owner.Email!,
                    TenantName = $"{s.Tenant.Owner.FirstName} {s.Tenant.Owner.LastName}",
                    EndsAt = s.EndsAt,
                    SubDomain = s.Tenant.SubDomain
                }).ToListAsync(cancellationToken);
        }
        public async Task BulkExpireSubscriptionsAsync(List<int> subscriptionIds, CancellationToken cancellationToken)
        {
            await _context.Subscriptions
                .Where(s => subscriptionIds.Contains(s.Id))
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.Status, SubscriptionStatus.Expired), cancellationToken);
        }
    }
}