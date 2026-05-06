using Microsoft.AspNetCore.Http;

namespace Application.Features.ModuleItems.Commands.DeleteModuleItem
{
    internal sealed class DeleteModuleItemCommandHandler : IRequestHandler<DeleteModuleItemCommand, OneOf<SuccessDto, Error>>
    {
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IModuleItemRepository _moduleItemRepository;
        private readonly HybridCache _hybridCache;
        public DeleteModuleItemCommandHandler(ITenantMemberRepository tenantMemberRepository, ICurrentUserId currentUserId,
            ISubscriptionRepository subscriptionRepository, IHttpContextAccessor httpContextAccessor, IModuleItemRepository moduleItemRepository,
            HybridCache hybridCache)
        {
            _tenantMemberRepository = tenantMemberRepository;
            _currentUserId = currentUserId;
            _subscriptionRepository = subscriptionRepository;
            _httpContextAccessor = httpContextAccessor;
            _moduleItemRepository = moduleItemRepository;
            _hybridCache = hybridCache;
        }
        public async Task<OneOf<SuccessDto, Error>> Handle(DeleteModuleItemCommand request, CancellationToken cancellationToken)
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
            var moduleItem = await _moduleItemRepository.GetAsync(request.ItemId, request.ModuleId, request.CourseId, subdomain!, cancellationToken);
            if (moduleItem is null)
            {
                return ModuleItemErrors.ModuleItemNotFound;
            }
            await _moduleItemRepository.RemoveAsync(moduleItem, cancellationToken);
            await _hybridCache.RemoveByTagAsync(tags: new[] { $"{CacheKeysConstants.AllCoursesKey}_{request.CourseId}" }, cancellationToken);
            return new SuccessDto
            {
                Id = moduleItem.Id.ToString(),
                Message = $"{nameof(ModuleItem)} {SuccessConstants.ItemDeleted}"
            };
        }
    }
}
