namespace Application.Contracts.Repositories
{
    public interface IRefreshRepository
    {
        Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken);
        void AddRefreshToken(ApplicationUser user, string token, DateTime expiresAt, CancellationToken cancellationToken);

    }
}
