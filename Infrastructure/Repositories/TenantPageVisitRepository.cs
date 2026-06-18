using Application.Features.Public.Dtos;
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

        public async Task<PublicStatisticsDto> GetTenantStatisticsAsync(string subDomain, CancellationToken cancellationToken)
        {
            var visitsTask = _context.TenantPageVisits
                .Where(v => v.Tenant.SubDomain == subDomain)
                .Select(v => new VisitRow(v.VisitorId, v.Views, v.Converted, v.DurationSecond, v.VisitedAt, v.DeviceType))
                .ToListAsync(cancellationToken);

            var topPagesTask = GetTopPagesAsync(subDomain, cancellationToken);
            var monthlyRevenueTask = GetMonthlyRevenueAsync(subDomain, cancellationToken);

            await Task.WhenAll(visitsTask, topPagesTask, monthlyRevenueTask);

            var visits = await visitsTask;

            return new PublicStatisticsDto
            {
                WebsiteScorecards = BuildWebsiteScorecards(visits),
                VisitorsAndPageViewsData = BuildVisitorsAndPageViewsData(visits),
                DeviceDistributionData = BuildDeviceDistribution(visits),
                TopPagesData = await topPagesTask,
                MonthlyRevenueData = await monthlyRevenueTask
            };
        }
        private sealed record VisitRow(Guid VisitorId, int Views, bool Converted, int? DurationSecond, DateTime VisitedAt, DeviceType DeviceType);
        private static WebsiteScorecardsDto BuildWebsiteScorecards(List<VisitRow> visits)
        {
            if (visits.Count == 0)
                return new WebsiteScorecardsDto();

            var totalVisitors = visits.Select(v => v.VisitorId).Distinct().Count();
            var totalPageViews = visits.Sum(v => v.Views);

            var convertedVisitors = visits
                .Where(v => v.Converted)
                .Select(v => v.VisitorId)
                .Distinct()
                .Count();

            var conversionRate = totalVisitors > 0
                ? convertedVisitors * 100.0 / totalVisitors
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
        private static List<VisitorsAndPageViewsDataDto> BuildVisitorsAndPageViewsData(List<VisitRow> visits)
        {
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
        private static List<DeviceDistributionDto> BuildDeviceDistribution(List<VisitRow> visits)
        {
            return visits
                .GroupBy(v => v.DeviceType)
                .Select(g => new DeviceDistributionDto
                {
                    DeviceType = g.Key,
                    Visitors = g.Select(v => v.VisitorId).Distinct().Count()
                }).ToList();
        }
        private async Task<List<TopPagesDto>> GetTopPagesAsync(string subDomain, CancellationToken cancellationToken)
        {
            return await _context.TenantPages
                .Where(p => p.Tenant.SubDomain == subDomain)
                .OrderByDescending(p => p.Views)
                .Take(8)
                .Select(p => new TopPagesDto
                {
                    Page = p.Url,
                    Views = p.Views
                }).ToListAsync(cancellationToken);
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