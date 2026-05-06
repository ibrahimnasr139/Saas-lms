using Microsoft.AspNetCore.Http;

namespace Application.Features.Modules.Commands.UpdateModule
{
    internal sealed class UpdateModuleCommandHandler : IRequestHandler<UpdateModuleCommand, OneOf<SuccessDto, Error>>
    {
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IModuleRepository _moduleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICourseRepository _courseRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public UpdateModuleCommandHandler(ITenantMemberRepository tenantMemberRepository, ICurrentUserId currentUserId,
            ISubscriptionRepository subscriptionRepository, IHttpContextAccessor httpContextAccessor, ICourseRepository courseRepository,
            IMapper mapper, IModuleRepository moduleRepository, IUnitOfWork unitOfWork)
        {
            _tenantMemberRepository = tenantMemberRepository;
            _currentUserId = currentUserId;
            _subscriptionRepository = subscriptionRepository;
            _httpContextAccessor = httpContextAccessor;
            _courseRepository = courseRepository;
            _mapper = mapper;
            _moduleRepository = moduleRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<SuccessDto, Error>> Handle(UpdateModuleCommand request, CancellationToken cancellationToken)
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
            _mapper.Map(request, module);
            var maxSize = await _moduleRepository.GetMaxOrder(request.CourseId, cancellationToken);
            if (module.Order > maxSize)
            {
                module.Order = maxSize;
            }
            if (oldOrder > module.Order)
            {
                await _moduleRepository.IncreaseOrder(module.Id, request.CourseId, module.Order, cancellationToken, oldOrder);
            }
            else if (oldOrder < module.Order)
            {
                await _moduleRepository.DecreaseOrder(module.Id, request.CourseId, oldOrder, cancellationToken, module.Order);
            }
            await _unitOfWork.SaveAsync(cancellationToken);

            return new SuccessDto
            {
                Id = module.Id.ToString(),
                Message = $"{nameof(Module)} {SuccessConstants.ItemUpdated}"
            };
        }
    }
}
