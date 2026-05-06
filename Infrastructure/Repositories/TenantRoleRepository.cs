using Application.Constants;
using Application.Features.Roles.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Infrastructure.Repositories
{
    internal sealed class TenantRoleRepository : ITenantRoleRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TenantRoleRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<TenantRolesDto>> GetTenantRolesAsync(int tenantId, CancellationToken cancellationToken)
        {
            return await _context.TenantRoles
               .AsNoTracking()
               .Where(tr => tr.TenantId == tenantId)
               .ProjectTo<TenantRolesDto>(_mapper.ConfigurationProvider)
               .ToListAsync(cancellationToken);
        }
        public Task<string?> GetRoleNameAsync(int roleId, CancellationToken cancellationToken)
        {
            return _context.TenantRoles
                .AsNoTracking()
                .Where(tr => tr.Id == roleId)
                .Select(tr => tr.Name)
                .FirstOrDefaultAsync(cancellationToken);
        }
        public Task<bool> IsOwnerAsync(int roleId, CancellationToken cancellationToken)
        {
            return _context.TenantRoles
                .AsNoTracking()
                .Where(tr => tr.Id == roleId)
                .AnyAsync(tr => tr.Name == TenantRoleConstants.Owner, cancellationToken);
        }
        public async Task UpdateRoleAsync(int roleId, string name, string description, List<string> enabledPermissions, CancellationToken cancellationToken)
        {
            var role = await _context.TenantRoles
                .Include(tr => tr.RolePermissions)
                .FirstOrDefaultAsync(tr => tr.Id == roleId, cancellationToken);

            var permissionsToRemove = role!.RolePermissions.Where(rp => !enabledPermissions.Contains(rp.PermissionId)).ToList();
            if (permissionsToRemove.Any())
                _context.RolePermissions.RemoveRange(permissionsToRemove);

            var existingPermissionIds = role.RolePermissions.Select(rp => rp.PermissionId).ToHashSet();
            var permissionsToAdd = enabledPermissions.Where(ep => !existingPermissionIds.Contains(ep))
                .Select(ep => new RolePermission
                {
                    TenantRoleId = roleId,
                    PermissionId = ep
                }).ToList();

            if (permissionsToAdd.Any())
                await _context.RolePermissions.AddRangeAsync(permissionsToAdd, cancellationToken);

            role.Name = name;
            role.Description = description;
            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task DeleteRoleAsync(int roleId, CancellationToken cancellationToken)
        {
            await _context.TenantRoles
                .Where(tr => tr.Id == roleId)
                .ExecuteDeleteAsync(cancellationToken);
        }
        public async Task CreateRoleASync(TenantRole tenantRole, CancellationToken cancellationToken)
        {
            await _context.TenantRoles.AddAsync(tenantRole);
            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task AddRolePermissionsAsync(int tenantRoleId, List<string> enabledPermissions, CancellationToken cancellationToken)
        {
            var rolePermissions = enabledPermissions.Select(permission => new RolePermission
            {
                TenantRoleId = tenantRoleId,
                PermissionId = permission
            });

            await _context.RolePermissions.AddRangeAsync(rolePermissions, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
