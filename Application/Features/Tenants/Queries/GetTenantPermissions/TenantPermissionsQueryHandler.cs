using Application.Features.Tenants.Dtos;

namespace Application.Features.Tenants.Queries.GetTenantPermissions
{
    internal sealed class TenantPermissionsQueryHandler : IRequestHandler<TenantPermissionsQuery, List<TenantPermissionDto>>
    {
        private readonly IPermissionRepository _permissionRepository;

        public TenantPermissionsQueryHandler(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }
        public async Task<List<TenantPermissionDto>> Handle(TenantPermissionsQuery request, CancellationToken cancellationToken) =>
             await _permissionRepository.GetAllPermissionsAsync(cancellationToken);
    }
}
