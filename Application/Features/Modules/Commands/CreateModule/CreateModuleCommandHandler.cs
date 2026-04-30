using Microsoft.AspNetCore.Http;

namespace Application.Features.Modules.Commands.CreateModule
{
    internal sealed class CreateModuleCommandHandler : IRequestHandler<CreateModuleCommand, OneOf<SuccessDto, Error>>
    {
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IModuleRepository _moduleRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly HybridCache _hybridCache;
        public CreateModuleCommandHandler(ITenantMemberRepository tenantMemberRepository, ICurrentUserId currentUserId,
            ISubscriptionRepository subscriptionRepository, IHttpContextAccessor httpContextAccessor, ICourseRepository courseRepository,
            IMapper mapper, IModuleRepository moduleRepository, HybridCache hybridCache)
        {
            _tenantMemberRepository = tenantMemberRepository;
            _currentUserId = currentUserId;
            _subscriptionRepository = subscriptionRepository;
            _httpContextAccessor = httpContextAccessor;
            _courseRepository = courseRepository;
            _mapper = mapper;
            _moduleRepository = moduleRepository;
            _hybridCache = hybridCache;
        }
        public async Task<OneOf<SuccessDto, Error>> Handle(CreateModuleCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var isPermitted = await _tenantMemberRepository.IsPermittedMember(userId, PermissionConstants.MANAGE_MODULE_ITEMS, cancellationToken);
            if (!isPermitted)
                return MemberErrors.NotAllowed;

            var isSubscribed = await _subscriptionRepository.HasActiveSubscriptionByTenantDomain(subdomain!, cancellationToken);
            if (!isSubscribed)
                return TenantErrors.NotSubscribed;

            var course = await _courseRepository.GetCourseByIdAsync(request.CourseId, subdomain!, cancellationToken);
            if (course is null)
                return CourseErrors.CourseNotFound;

            var module = _mapper.Map<Module>(request);
            var maxOrder = await _moduleRepository.GetMaxOrder(course.Id, cancellationToken);
            if (request.Order > maxOrder + 1)
                module.Order = maxOrder + 1;

            var moduleId = await _moduleRepository.CreateModule(module, cancellationToken);
            if (request.Order <= maxOrder)
                await _moduleRepository.IncreaseOrder(moduleId, course.Id, request.Order, cancellationToken);

            await _hybridCache.RemoveAsync($"{CacheKeysConstants.CourseModuleKey}-{request.CourseId}", cancellationToken);
            await _hybridCache.RemoveAsync($"{CacheKeysConstants.CourseStatisticsKey}-{request.CourseId}", cancellationToken);
            return new SuccessDto
            {
                Id = moduleId.ToString(),
                Message = $"{nameof(Module)} {SuccessConstants.ItemCreated}"
            };
        }
    }
}
