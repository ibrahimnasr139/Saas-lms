using Application.Features.Roles.Dtos;

namespace Application.Features.Roles.Commands.UpdateRole
{
    internal sealed class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, OneOf<RoleDto, Error>>
    {
        private readonly ITenantRoleRepository _tenantRoleRepository;
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ICurrentUserId _currentUserId;

        public UpdateRoleCommandHandler(ITenantRoleRepository tenantRoleRepository, ITenantMemberRepository tenantMemberRepository,
            ICurrentUserId currentUserId)
        {
            _tenantRoleRepository = tenantRoleRepository;
            _tenantMemberRepository = tenantMemberRepository;
            _currentUserId = currentUserId;
        }
        public async Task<OneOf<RoleDto, Error>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var isPermitted = await _tenantMemberRepository.IsPermittedMember(userId, PermissionConstants.MANAGE_PERMISSIONS, cancellationToken);
            if (!isPermitted)
                return MemberErrors.NotAllowed;

            var isOwner = await _tenantRoleRepository.IsOwnerAsync(request.RoleId, cancellationToken);
            if (isOwner)
                return TenantMemberErrors.CannotChangeOwnerRole;

            await _tenantRoleRepository.UpdateRoleAsync(request.RoleId, request.Name, request.Description, request.EnabledPermissions, cancellationToken);
            return new RoleDto { Message = MessagesConstants.RoleUpdated };
        }
    }
}
