namespace Application.Contracts.Repositories
{
    public interface IZoomOAuthStateRepository
    {
        Task CreateAsync(ZoomOAuthState oauthState, CancellationToken cancellationToken);
        Task<ZoomOAuthState?> GetOAuthStateAsync(string state, CancellationToken cancellationToken);
        Task DeleteAllExpiredAndUsedStatesAsync();
        Task<ZoomOAuthState?> TryMarkAsUsedAsync(string state, CancellationToken cancellationToken);
    }
}