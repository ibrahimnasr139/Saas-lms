using Application.Features.TenantMembers.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TenantMembers.Commands.UpdateCurrentMember
{
    internal sealed class UpdateCurrentMemberCommandHandler : IRequestHandler<UpdateCurrentMemberCommand, UpdateCurrentMemberDto>
    {
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HybridCache _hybridCache;

        public UpdateCurrentMemberCommandHandler(ITenantMemberRepository tenantMemberRepository, ITenantRepository tenantRepository,
            ICurrentUserId currentUserId, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, HybridCache hybridCache)
        {
            _tenantMemberRepository = tenantMemberRepository;
            _tenantRepository = tenantRepository;
            _currentUserId = currentUserId;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _hybridCache = hybridCache;
        }
        public async Task<UpdateCurrentMemberDto> Handle(UpdateCurrentMemberCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);
            var memberId = await _tenantMemberRepository.GetMemberIdByUserIdAsync(userId, tenantId, cancellationToken);

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.FirstName = request.FirstName ?? user.FirstName;
                user.LastName = request.LastName ?? user.LastName;
                user.Email = request.Email ?? user.Email;
                user.PhoneNumber = request.Phone ?? user.PhoneNumber;
                user.ProfilePicture = request.ProfilePicture ?? user.ProfilePicture;
                await _userManager.UpdateAsync(user);
            }

            await _tenantMemberRepository.UpdateCurrentMemberAsync(tenantId, memberId, request, cancellationToken);
            await _hybridCache.RemoveAsync($"{CacheKeysConstants.CurrentTenantMemberKey}_{userId}", cancellationToken);
            await _hybridCache.RemoveAsync($"{CacheKeysConstants.LastTenantKey}_{userId}", cancellationToken);
            return new UpdateCurrentMemberDto { Message = MessagesConstants.TenantMemberUpdateCurrentMember };
        }
    }
}