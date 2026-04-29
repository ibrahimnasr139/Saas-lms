namespace Infrastructure.Repositories
{
    internal sealed class RefreshRepository : IRefreshRepository
    {
        private readonly AppDbContext _context;
        public RefreshRepository(AppDbContext context)
        {
            _context = context;
        }
        public async void AddRefreshToken(ApplicationUser user, string token, DateTime expiresAt, CancellationToken cancellationToken)
        {
            user.RefreshTokens.Add(new RefreshToken
            {
                Token = token,
                ExpiresAt = expiresAt
            });
        }
        public async Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
        }
    }
}
