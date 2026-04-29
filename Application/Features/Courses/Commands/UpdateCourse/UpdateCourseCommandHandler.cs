using Application.Contracts.Repositories;
using Application.Features.Courses.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Courses.Commands.UpdateCourse
{
    internal sealed class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, OneOf<SuccessDto, Error>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HybridCache _hybridCache;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCourseCommandHandler(ICourseRepository courseRepository,IMapper mapper,IUnitOfWork unitOfWork,
            ITenantMemberRepository tenantMemberRepository, ISubscriptionRepository subscriptionRepository,
            ICurrentUserId currentUserId,IHttpContextAccessor httpContextAccessor,HybridCache hybridCache)
        {
            _courseRepository = courseRepository;
            _mapper = mapper;
            _tenantMemberRepository = tenantMemberRepository;
            _subscriptionRepository = subscriptionRepository;
            _currentUserId = currentUserId;
            _httpContextAccessor = httpContextAccessor;
            _hybridCache = hybridCache;
            _unitOfWork = unitOfWork;   
        }

        public async Task<OneOf<SuccessDto, Error>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var isPermitted = await _tenantMemberRepository.IsPermittedMember(userId, PermissionConstants.EDIT_COURSES, cancellationToken);
            if (!isPermitted)
            {
                return MemberErrors.NotAllowed;
            }
            var subDomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var isSubscribed = await _subscriptionRepository.HasActiveSubscriptionByTenantDomain(subDomain!, cancellationToken);
            if (!isSubscribed)
            {
                return TenantErrors.NotSubscribed;
            }
            var course = await _courseRepository.GetCourseByIdAsync(request.CourseId, subDomain!, cancellationToken);
            if (course == null)
            {
                return CourseErrors.CourseNotFound;
            }
            _mapper.Map(request, course);
            await _unitOfWork.SaveAsync(cancellationToken);
            await _hybridCache.RemoveByTagAsync(tags: new[] { $"{CacheKeysConstants.AllCoursesKey}_{subDomain}" }, cancellationToken);
            return new SuccessDto
            {
                Id = course.Id.ToString(),
                Message = $"{nameof(Course)} {SuccessConstants.ItemUpdated}"
            };
        }
    }
}
