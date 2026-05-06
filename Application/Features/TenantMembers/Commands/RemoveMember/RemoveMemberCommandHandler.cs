using Application.Features.TenantMembers.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TenantMembers.Commands.RemoveMember
{
    internal sealed class RemoveMemberCommandHandler : IRequestHandler<RemoveMemberCommand, OneOf<RemoveMemberDto, Error>>
    {
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUserId _currentUserId;
        private readonly HybridCache _hybridCache;

        public RemoveMemberCommandHandler(
            ITenantMemberRepository tenantMemberRepository,
            ITenantRepository tenantRepository,
            IHttpContextAccessor httpContextAccessor,
            ICurrentUserId currentUserId, HybridCache hybridCache)
        {
            _tenantMemberRepository = tenantMemberRepository;
            _tenantRepository = tenantRepository;
            _httpContextAccessor = httpContextAccessor;
            _currentUserId = currentUserId;
            _hybridCache = hybridCache;
        }

        public async Task<OneOf<RemoveMemberDto, Error>> Handle(RemoveMemberCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var isPermitted = await _tenantMemberRepository.IsPermittedMember(userId, PermissionConstants.MANAGE_MEMBERS, cancellationToken);
            if (!isPermitted)
                return MemberErrors.NotAllowed;

            var member = await _tenantMemberRepository.GetMemberByIdAsync(request.MemberId, cancellationToken);
            if (member == null)
                return TenantMemberErrors.MemberNotFound;

            var isOwner = await _tenantMemberRepository.IsOwnerAsync(request.MemberId, cancellationToken);
            if (isOwner)
                return TenantMemberErrors.CannotRemoveOwner;

            if (member.UserId == userId)
                return TenantMemberErrors.CannotRemoveSelf;

            await _tenantMemberRepository.RemoveMemberAsync(request.MemberId, cancellationToken);

            var cacheKey = $"{CacheKeysConstants.CurrentTenantMemberKey}_{member.UserId}";
            await _hybridCache.RemoveAsync(cacheKey);
            return new RemoveMemberDto { Message = MessagesConstants.TenantMemberDeleted };
        }
    }
}
