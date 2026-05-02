using Microsoft.AspNetCore.Http;

namespace Application.Features.ModuleItems.Commands.UpdateSettings
{
    internal sealed class UpdateSettingsCommandHandler : IRequestHandler<UpdateSettingsCommand, OneOf<SuccessDto, Error>>
    {
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IModuleItemRepository _moduleItemRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSettingsCommandHandler(ITenantMemberRepository tenantMemberRepository, ICurrentUserId currentUserId,
            ISubscriptionRepository subscriptionRepository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper, IModuleItemRepository moduleItemRepository, IUnitOfWork unitOfWork)
        {
            _tenantMemberRepository = tenantMemberRepository;
            _currentUserId = currentUserId;
            _subscriptionRepository = subscriptionRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _moduleItemRepository = moduleItemRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<SuccessDto, Error>> Handle(UpdateSettingsCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var isPermitted = await _tenantMemberRepository.IsPermittedMember(userId, PermissionConstants.MANAGE_MODULE_ITEMS, cancellationToken);
            if (!isPermitted)
                return MemberErrors.NotAllowed;

            var isSubscribed = await _subscriptionRepository.HasActiveSubscriptionByTenantDomain(subdomain!, cancellationToken);
            if (!isSubscribed)
                return TenantErrors.NotSubscribed;

            var moduleItem = await _moduleItemRepository.GetItemConditions(request.ItemId, request.ModuleId, request.CourseId, subdomain!, cancellationToken);
            if (moduleItem is null)
                return ModuleItemErrors.ModuleItemNotFound;

            moduleItem.Conditions.Clear();
            _mapper.Map(request, moduleItem);
            var mappedConditions = _mapper.Map<IEnumerable<ModuleItemCondition>>(request.Conditions);
            foreach (var condition in mappedConditions)
            {
                condition.ModuleItemId = moduleItem.Id;
                moduleItem.Conditions.Add(condition);
            }
            await _unitOfWork.SaveAsync(cancellationToken);
            return new SuccessDto
            {
                Id = moduleItem.Id.ToString(),
                Message = $"{nameof(ModuleItem)} {SuccessConstants.ItemUpdated}"
            };
        }
    }
}
