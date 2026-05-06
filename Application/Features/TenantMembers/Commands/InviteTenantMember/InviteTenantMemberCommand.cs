using Application.Features.TenantMembers.Dtos;

namespace Application.Features.TenantMembers.Commands.InviteTenantMember
{
    public sealed record InviteTenantMemberCommand(int roleId, string email) :
        IRequest<OneOf<InviteTenantMemberDto, Error>>;
}
