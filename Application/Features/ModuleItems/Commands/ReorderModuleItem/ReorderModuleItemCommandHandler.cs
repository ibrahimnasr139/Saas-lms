using Microsoft.AspNetCore.Http;

namespace Application.Features.ModuleItems.Commands.ReorderModuleItem
{
    internal sealed class ReorderModuleItemCommandHandler : IRequestHandler<ReorderModuleItemCommand, OneOf<bool, Error>>
    {
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IModuleItemRepository _moduleItemRepository;
        private readonly IModuleRepository _moduleRepository;
        public ReorderModuleItemCommandHandler(ITenantMemberRepository tenantMemberRepository, ICurrentUserId currentUserId,
            ISubscriptionRepository subscriptionRepository, IHttpContextAccessor httpContextAccessor,
            IModuleItemRepository moduleItemRepository, IModuleRepository moduleRepository)
        {
            _tenantMemberRepository = tenantMemberRepository;
            _currentUserId = currentUserId;
            _subscriptionRepository = subscriptionRepository;
            _httpContextAccessor = httpContextAccessor;
            _moduleItemRepository = moduleItemRepository;
            _moduleRepository = moduleRepository;
        }
        public async Task<OneOf<bool, Error>> Handle(ReorderModuleItemCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var isPermitted = await _tenantMemberRepository.IsPermittedMember(userId, PermissionConstants.MANAGE_MODULE_ITEMS, cancellationToken);
            if (!isPermitted)
            {
                return MemberErrors.NotAllowed;
            }
            var isSubscribed = await _subscriptionRepository.HasActiveSubscriptionByTenantDomain(subdomain!, cancellationToken);
            if (!isSubscribed)
            {
                return TenantErrors.NotSubscribed;
            }
            var module = await _moduleRepository.GetModuleByIdAsync(request.ModuleId, request.CourseId, subdomain!, cancellationToken);
            if (module is null)
            {
                return ModuleErrors.ModuleNotFound;
            }
            await _moduleItemRepository.ReorderItems(request.Orders, cancellationToken);
            return true;
        }
    }
}
