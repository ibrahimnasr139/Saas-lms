using Application.Features.TenantMembers.Dtos;

namespace Application.Contracts.Repositories
{
    public interface ITenantInviteRepository
    {
        Task CreateTenantInviteAsync(TenantInvite tenantInvite, CancellationToken cancellationToken);
        Task<ValidateTenanInviteDto> GetValidateTenanInviteAsync(string token, CancellationToken cancellationToken);
        Task<bool> IsValidTokenAsync(string token, CancellationToken cancellationToken);
        Task<string> GetInvitedMemberEmailAsync(string token, CancellationToken cancellationToken);
        Task<TenantInvite?> GetInviteByTokenAsync(string token, CancellationToken cancellationToken);
        Task AcceptInviteAsync(string token, CancellationToken cancellationToken);
        Task DeclineInviteAsync(string token, CancellationToken cancellationToken);
    }
}
