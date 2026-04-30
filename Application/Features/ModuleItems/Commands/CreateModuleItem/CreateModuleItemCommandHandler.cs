using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Features.ModuleItems.Commands.CreateModuleItem
{
    internal sealed class CreateModuleItemCommandHandler : IRequestHandler<CreateModuleItemCommand, OneOf<SuccessDto, Error>>
    {
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IModuleRepository _moduleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IModuleItemRepository _moduleItemRepository;
        private readonly IMapper _mapper;
        private readonly HybridCache _hybridCache;
        private readonly IUnitOfWork _unitOfWork;
        public CreateModuleItemCommandHandler(ITenantMemberRepository tenantMemberRepository, ICurrentUserId currentUserId,
            ISubscriptionRepository subscriptionRepository, IHttpContextAccessor httpContextAccessor, IMapper mapper, IModuleRepository moduleRepository,
            IModuleItemRepository moduleItemRepository,
            HybridCache hybridCache, IUnitOfWork unitOfWork)
        {
            _tenantMemberRepository = tenantMemberRepository;
            _currentUserId = currentUserId;
            _subscriptionRepository = subscriptionRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _moduleRepository = moduleRepository;
            _moduleItemRepository = moduleItemRepository;
            _hybridCache = hybridCache;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<SuccessDto, Error>> Handle(CreateModuleItemCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var isPermitted = await _tenantMemberRepository.IsPermittedMember(userId, PermissionConstants.MANAGE_MODULE_ITEMS, cancellationToken);
            if (!isPermitted)
                return MemberErrors.NotAllowed;

            var isSubscribed = await _subscriptionRepository.HasActiveSubscriptionByTenantDomain(subdomain!, cancellationToken);
            if (!isSubscribed)
                return TenantErrors.NotSubscribed;

            var module = await _moduleRepository.GetModuleByIdAsync(request.ModuleId, request.CourseId, subdomain!, cancellationToken);
            if (module is null)
                return ModuleErrors.ModuleNotFound;

            var moduleItem = _mapper.Map<ModuleItem>(request);
            var maxOrder = await _moduleItemRepository.GetMaxOrder(request.CourseId, request.ModuleId, cancellationToken);
            moduleItem.Order = maxOrder + 1;

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var moduleItemId = await _moduleItemRepository.CreateModuleItem(moduleItem, cancellationToken);
                if (request.Type == ModuleItemType.Lesson)
                {
                    var lesson = _mapper.Map<Lesson>(request, opt =>
                    {
                        opt.AfterMap((src, dest) =>
                        {
                            dest.ModuleItemId = moduleItemId;
                        });
                    });
                    await _moduleItemRepository.CreateLesson(lesson, cancellationToken);
                }
                else if (request.Type == ModuleItemType.Assignment)
                {
                    var assignment = _mapper.Map<Assignment>(request, opt =>
                    {
                        opt.AfterMap((src, dest) =>
                        {
                            dest.ModuleItemId = moduleItemId;
                            dest.CreatedById = userId;
                        });
                    });
                    await _moduleItemRepository.CreateAssignment(assignment, cancellationToken);
                }
                else
                {
                    var quiz = _mapper.Map<Quiz>(request, opt =>
                    {
                        opt.AfterMap((src, dest) =>
                        {
                            dest.ModuleItemId = moduleItemId;
                            dest.CreatedById = userId;
                        });
                    });
                    await _moduleItemRepository.CreateQuiz(quiz, cancellationToken);
                }
                await _hybridCache.RemoveByTagAsync(tags: new[] { $"{CacheKeysConstants.AllCoursesKey}_{request.CourseId}" }, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return new SuccessDto
                {
                    Id = moduleItemId.ToString(),
                    Message = $"{nameof(ModuleItem)} {SuccessConstants.ItemCreated}"
                };
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}