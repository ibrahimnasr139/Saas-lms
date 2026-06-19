using Application.Features.Website.Dtos;
using Domain.Enums;

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
        public async Task<WebSiteStatisticsDto> GetTenantStatisticsAsync(string subDomain, CancellationToken cancellationToken)
        {
            var websiteScorecards = await GetWebsiteScorecardsAsync(subDomain, cancellationToken);
            var visitorsAndPageViewsData = await GetVisitorsAndPageViewsDataAsync(subDomain, cancellationToken);
            var topPagesData = await GetTopPagesAsync(subDomain, cancellationToken);
            var deviceDistributionData = await GetDeviceDistributionAsync(subDomain, cancellationToken);
            var monthlyRevenueData = await GetMonthlyRevenueAsync(subDomain, cancellationToken);

            return new WebSiteStatisticsDto
            {
                WebsiteScorecards = websiteScorecards,
                VisitorsAndPageViewsData = visitorsAndPageViewsData,
                TopPagesData = topPagesData,
                DeviceDistributionData = deviceDistributionData,
                MonthlyRevenueData = monthlyRevenueData
            };
        }
        private async Task<WebsiteScorecardsDto> GetWebsiteScorecardsAsync(string subDomain, CancellationToken cancellationToken)
        {
            var visits = await _context.TenantPageVisits
                .Where(v => v.Tenant.SubDomain == subDomain)
                .Select(v => new { v.VisitorId, v.Views, v.Converted, v.DurationSecond })
                .ToListAsync(cancellationToken);

            if (visits.Count == 0)
                return new WebsiteScorecardsDto();

            var uniqueVisitorIds = visits.Select(v => v.VisitorId).Distinct().ToList();
            var totalVisitors = uniqueVisitorIds.Count;
            var totalPageViews = visits.Sum(v => v.Views);

            var convertedUniqueVisitors = visits
                .Where(v => v.Converted)
                .Select(v => v.VisitorId)
                .Distinct()
                .Count();

            var conversionRate = totalVisitors > 0
                ? convertedUniqueVisitors * 100.0 / totalVisitors
                : 0.0;

            var avgSessionDuration = visits
                .Where(v => v.DurationSecond.HasValue)
                .Select(v => (double)v.DurationSecond!.Value)
                .DefaultIfEmpty(0.0)
                .Average();

            return new WebsiteScorecardsDto
            {
                Visitors = totalVisitors,
                PageViews = totalPageViews,
                ConversionRate = conversionRate,
                AverageSessionDuration = avgSessionDuration
            };
        }
        private async Task<List<VisitorsAndPageViewsDataDto>> GetVisitorsAndPageViewsDataAsync(string subDomain, CancellationToken cancellationToken)
        {
            var visits = await _context.TenantPageVisits
                .Where(v => v.Tenant.SubDomain == subDomain)
                .Select(v => new { v.VisitorId, v.Views, v.VisitedAt })
                .ToListAsync(cancellationToken);

            return visits
                .GroupBy(v => new { v.VisitedAt.Year, v.VisitedAt.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new VisitorsAndPageViewsDataDto
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM"),
                    Visitors = g.Select(v => v.VisitorId).Distinct().Count(),
                    PageViews = g.Sum(v => v.Views)
                }).ToList();
        }
        private async Task<List<TopPagesDto>> GetTopPagesAsync(string subDomain, CancellationToken cancellationToken)
        {
            return await _context.TenantPageVisits
                .Where(v => v.Tenant.SubDomain == subDomain)
                .GroupBy(v => v.PageUrl)
                .Select(g => new TopPagesDto
                {
                    Page = g.Key,
                    Views = g.Sum(v => v.Views)
                })
                .OrderByDescending(p => p.Views)
                .Take(5)
                .ToListAsync(cancellationToken);
        }
        private async Task<List<DeviceDistributionDto>> GetDeviceDistributionAsync(string subDomain, CancellationToken cancellationToken)
        {
            var visits = await _context.TenantPageVisits
                .Where(v => v.Tenant.SubDomain == subDomain)
                .Select(v => new { v.VisitorId, v.DeviceType })
                .ToListAsync(cancellationToken);

            return visits
                .GroupBy(v => v.DeviceType)
                .Select(g => new DeviceDistributionDto
                {
                    DeviceType = g.Key,
                    Visitors = g.Select(v => v.VisitorId).Distinct().Count(),
                })
                .ToList();
        }
        private async Task<List<MonthlyRevenueDto>> GetMonthlyRevenueAsync(string subDomain, CancellationToken cancellationToken)
        {
            var tenantCreatedAt = await _context.Tenants
                .Where(t => t.SubDomain == subDomain)
                .Select(t => t.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            var sixMonthsAgo = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-5);
            var startDate = tenantCreatedAt > sixMonthsAgo
                ? new DateTime(tenantCreatedAt.Year, tenantCreatedAt.Month, 1, 0, 0, 0, DateTimeKind.Utc)
                : sixMonthsAgo;

            var revenueData = await _context.Orders
                .Where(o => o.Tenant.SubDomain == subDomain && o.CreatedAt >= startDate)
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Revenue = g.Sum(o => (decimal?)o.PricePaid) ?? 0
                })
                .ToListAsync(cancellationToken);

            var allMonths = Enumerable.Range(0, 6)
                .Select(i => DateTime.UtcNow.AddMonths(-5 + i))
                .Select(d => new { d.Year, d.Month })
                .Where(m => new DateTime(m.Year, m.Month, 1, 0, 0, 0, DateTimeKind.Utc) >= startDate)
                .OrderBy(m => m.Year)
                .ThenBy(m => m.Month)
                .ToList();

            return allMonths.Select(m =>
            {
                var existing = revenueData.FirstOrDefault(x => x.Year == m.Year && x.Month == m.Month);
                return new MonthlyRevenueDto
                {
                    Month = new DateTime(m.Year, m.Month, 1).ToString("MMM"),
                    Revenue = (int)(existing?.Revenue ?? 0)
                };
            }).ToList();
        }
    }
}