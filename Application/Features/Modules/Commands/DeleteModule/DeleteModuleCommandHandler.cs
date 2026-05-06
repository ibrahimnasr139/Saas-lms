using Microsoft.AspNetCore.Http;

namespace Application.Features.Modules.Commands.DeleteModule
{
    internal sealed class DeleteModuleCommandHandler : IRequestHandler<DeleteModuleCommand, OneOf<SuccessDto, Error>>
    {
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IModuleRepository _moduleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HybridCache _hybridCache;
        public DeleteModuleCommandHandler(ITenantMemberRepository tenantMemberRepository, ICurrentUserId currentUserId,
            ISubscriptionRepository subscriptionRepository, IHttpContextAccessor httpContextAccessor,
            IModuleRepository moduleRepository, HybridCache hybridCache)
        {
            _tenantMemberRepository = tenantMemberRepository;
            _currentUserId = currentUserId;
            _subscriptionRepository = subscriptionRepository;
            _httpContextAccessor = httpContextAccessor;
            _moduleRepository = moduleRepository;
            _hybridCache = hybridCache;
        }
        public async Task<OneOf<SuccessDto, Error>> Handle(DeleteModuleCommand request, CancellationToken cancellationToken)
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
            var oldOrder = module.Order;
            await _moduleRepository.RemoveModule(module, cancellationToken);
            await _moduleRepository.DecreaseOrder(module.Id, request.CourseId, oldOrder, cancellationToken);
            await _hybridCache.RemoveAsync($"{CacheKeysConstants.CourseStatisticsKey}-{request.CourseId}", cancellationToken);
            return new SuccessDto
            {
                Id = module.Id.ToString(),
                Message = $"{nameof(Module)} {SuccessConstants.ItemDeleted}"
            };
        }
    }
}
