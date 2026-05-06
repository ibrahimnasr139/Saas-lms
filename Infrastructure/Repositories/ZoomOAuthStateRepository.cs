namespace Infrastructure.Repositories
{
    internal sealed class ZoomOAuthStateRepository : IZoomOAuthStateRepository
    {
        private readonly AppDbContext _context;

        public ZoomOAuthStateRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task CreateAsync(ZoomOAuthState oauthState, CancellationToken cancellationToken)
        {
            await _context.ZoomOAuthStates.AddAsync(oauthState, cancellationToken);
        }
        public async Task<ZoomOAuthState?> GetOAuthStateAsync(string state, CancellationToken cancellationToken)
        {
            return await _context.ZoomOAuthStates
                .FromSqlRaw(@"
                    SELECT * FROM ""ZoomOAuthStates"" 
                    WHERE ""StateToken"" = {0} 
                    AND NOT ""IsUsed"" 
                    AND ""ExpiresAt"" > now()
                    LIMIT 1
                    FOR UPDATE SKIP LOCKED", state)
                .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<ZoomOAuthState?> TryMarkAsUsedAsync(string state, CancellationToken cancellationToken)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var oauthState = await GetOAuthStateAsync(state, cancellationToken);
                if (oauthState is null)
                    return null;

                oauthState.IsUsed = true;
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return oauthState;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                return null;
            }
        }
        public async Task DeleteAllExpiredAndUsedStatesAsync()
        {
            await _context.ZoomOAuthStates
                .Where(x => x.IsUsed || x.ExpiresAt < DateTime.UtcNow)
                .ExecuteDeleteAsync();
        }
    }
}