using Application.Features.Roles.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Roles.Commands.CreateRole
{
    internal sealed class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, OneOf<RoleDto, Error>>
    {
        private readonly ITenantRoleRepository _tenantRoleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly ICurrentUserId _currentUserId;

        public CreateRoleCommandHandler(ITenantRoleRepository tenantRoleRepository, IHttpContextAccessor httpContextAccessor,
            ITenantMemberRepository tenantMemberRepository, ITenantRepository tenantRepository, ICurrentUserId currentUserId)
        {
            _tenantRoleRepository = tenantRoleRepository;
            _httpContextAccessor = httpContextAccessor;
            _tenantMemberRepository = tenantMemberRepository;
            _tenantRepository = tenantRepository;
            _currentUserId = currentUserId;
        }
        public async Task<OneOf<RoleDto, Error>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var isPermitted = await _tenantMemberRepository.IsPermittedMember(userId, PermissionConstants.MANAGE_PERMISSIONS, cancellationToken);
            if (!isPermitted)
                return MemberErrors.NotAllowed;

            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);
            var tenantRole = new TenantRole
            {
                TenantId = tenantId,
                Name = request.Name,
                Description = request.Description
            };
            await _tenantRoleRepository.CreateRoleASync(tenantRole, cancellationToken);
            await _tenantRoleRepository.AddRolePermissionsAsync(tenantRole.Id, request.EnabledPermissions, cancellationToken);
            return new RoleDto { Message = MessagesConstants.RoleCreated };
        }
    }
}
