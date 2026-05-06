using Application.Features.Roles.Dtos;

namespace Application.Contracts.Repositories
{
    public interface ITenantRoleRepository
    {
        Task<List<TenantRolesDto>> GetTenantRolesAsync(int tenantId, CancellationToken cancellationToken);
        Task<string?> GetRoleNameAsync(int roleId, CancellationToken cancellationToken);
        Task<bool> IsOwnerAsync(int roleId, CancellationToken cancellationToken);
        Task UpdateRoleAsync(int roleId, string name, string description, List<string> enabledPermissions, CancellationToken cancellationToken);
        Task DeleteRoleAsync(int roleId, CancellationToken cancellationToken);
        Task CreateRoleASync(TenantRole tenantRole, CancellationToken cancellationToken);
        Task AddRolePermissionsAsync(int tenantRoleId, List<string> enabledPermissions, CancellationToken cancellationToken);
    }
}
