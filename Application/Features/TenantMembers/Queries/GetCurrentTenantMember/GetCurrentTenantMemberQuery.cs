using Application.Features.TenantMembers.Dtos;

namespace Application.Features.TenantMembers.Queries.GetCurrentTenantMember
{
    public record GetCurrentTenantMemberQuery : IRequest<CurrentTenantMemberDto>;
}