namespace Infrastructure.Repositories
{
    internal sealed class TenantPageVisitRepository : ITenantPageVisitRepository
    {
        private readonly AppDbContext _context;

        public TenantPageVisitRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddTenantPageVisitAsync(TenantPageVisit pageVisit, CancellationToken cancellationToken)
        {
            await _context.TenantPageVisits.AddAsync(pageVisit, cancellationToken);
        }
        public Task<TenantPageVisit?> GetTenantPageVisitAsync(string subDomain, string pageUrl, Guid visitorId, CancellationToken cancellationToken)
        {
            return _context.TenantPageVisits
                .Where(v => v.PageUrl == pageUrl && v.VisitorId == visitorId && v.Tenant.SubDomain == subDomain)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}