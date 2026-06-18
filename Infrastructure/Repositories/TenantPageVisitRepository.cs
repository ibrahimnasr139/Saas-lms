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
    }
}