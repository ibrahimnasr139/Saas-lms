using Application.Features.TenantMembers.Commands.UpdateCurrentMember;
using Application.Features.TenantMembers.Dtos;

namespace Application.Contracts.Repositories
{
    public interface ITenantMemberRepository
    {
        Task<List<string>?> GetAllPermissions(int tenantRoleId, CancellationToken cancellationToken);
        Task<CurrentTenantMemberDto?> GetCurrentTenantMemberAsync(string userId, CancellationToken cancellationToken);
        Task<List<TenantMembersDto>> GetTenantMembersAsync(int tenantId, CancellationToken cancellationToken);
        Task<bool> IsPermittedMember(string userId, string permission, CancellationToken cancellationToken);
        Task<List<int>> GetTenantIdsAsync(string userId, CancellationToken cancellationToken);
        Task<int> GetMemberIdByUserIdAsync(string userId, int tenantId, CancellationToken cancellationToken);
        Task<TenantMember?> GetMemberByIdAsync(int memberId, CancellationToken cancellationToken);
        Task<bool> IsOwnerAsync(int memberId, CancellationToken cancellationToken);
        Task RemoveMemberAsync(int memberId, CancellationToken cancellationToken);
        Task UpdateRoleMemberAsync(int memberId, int roleId, CancellationToken cancellationToken);
        Task<MemberProfileDto> GetMemberProfileAsync(int memberId, CancellationToken cancellationToken);
        Task UpdateCurrentMemberAsync(int tenantId, int memberId, UpdateCurrentMemberCommand updateCurrentMember, CancellationToken cancellationToken);
        Task<int> GetTenantmemberIdAsync(int tenantId, CancellationToken cancellationToken);
    }
}
