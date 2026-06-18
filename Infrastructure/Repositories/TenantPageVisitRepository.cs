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
        public async Task<TenantPageVisit?> GetByVisitorAndPageAsync(Guid visitorId, int tenantId, string pageUrl, CancellationToken cancellationToken)
        {
            return await _context.TenantPageVisits
                .FirstOrDefaultAsync(x => x.VisitorId == visitorId && x.TenantId == tenantId && x.PageUrl == pageUrl, cancellationToken);
        }
        //public async Task<PublicStatisticsDto> GetTenantStatisticsAsync(string subDomain, CancellationToken cancellationToken)
        //{
        //    var websiteScorecards = await _context.TenantPageVisits
        //        .Where(v => v.Tenant.SubDomain == subDomain)
        //        .GroupBy(v => v.TenantId)
        //        .Select(g => new WebsiteScorecardsDto
        //        {
        //            Visitors = g.Select(v => v.VisitorId).Distinct().Count(),
        //            PageViews = g.Count(),
        //            ConversionRate = g.Count() > 0 ? (g.Count(v => v.IsConversion) * 100.0 / g.Count()) : 0.0,
        //        })
        //        .FirstOrDefaultAsync(cancellationToken);

        //    return new PublicStatisticsDto
        //    {
        //        WebsiteScorecards = websiteScorecards
        //    };
        //}
    }
}