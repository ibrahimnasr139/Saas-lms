using Application.Features.TenantMembers.Dtos;

namespace Application.Features.TenantMembers.Queries.GetMemberProfile
{
    internal sealed class GetMemberProfileQueryHandler : IRequestHandler<GetMemberProfileQuery, OneOf<MemberProfileDto, Error>>
    {
        private readonly ITenantMemberRepository _tenantMemberRepository;

        public GetMemberProfileQueryHandler(ITenantMemberRepository tenantMemberRepository)
        {
            _tenantMemberRepository = tenantMemberRepository;
        }
        public async Task<OneOf<MemberProfileDto, Error>> Handle(GetMemberProfileQuery request, CancellationToken cancellationToken)
        {
            var member = await _tenantMemberRepository.GetMemberByIdAsync(request.MemberId, cancellationToken);
            if (member == null)
                return TenantMemberErrors.MemberNotFound;

            return await _tenantMemberRepository.GetMemberProfileAsync(request.MemberId, cancellationToken);
        }
    }
}
