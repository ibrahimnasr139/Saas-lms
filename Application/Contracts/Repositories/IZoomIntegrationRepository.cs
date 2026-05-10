using Application.Features.ZoomIntegration.Dtos;

namespace Application.Contracts.Repositories
{
    public interface IZoomIntegrationRepository
    {
        Task CreateAsync(ZoomIntegration zoomIntegration, CancellationToken cancellationToken);
        Task SaveOrUpdateIntegrationAsync(string userId, int tenantId, ZoomTokenResponse zoomToken, ZoomUserResponse zoomUser, CancellationToken cancellationToken);
        Task<ZoomIntegration?> GetZoomIntegrationAsync(string userId, string subDomain, CancellationToken cancellationToken);
    }
}
