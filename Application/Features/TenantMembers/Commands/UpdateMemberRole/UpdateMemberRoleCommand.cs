using Application.Features.TenantMembers.Dtos;

namespace Application.Features.TenantMembers.Commands.UpdateMemberRole
{
    public sealed record UpdateMemberRoleCommand(int MemberId, int RoleId) : IRequest<OneOf<UpdateMemberRoleDto, Error>>;
}
