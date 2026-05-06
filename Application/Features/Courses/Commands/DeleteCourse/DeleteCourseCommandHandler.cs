using Microsoft.AspNetCore.Http;

namespace Application.Features.Courses.Commands.DeleteCourse
{
    internal sealed class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand, OneOf<SuccessDto, Error>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteCourseCommandHandler(ICourseRepository courseRepository, ITenantMemberRepository tenantMemberRepository, ICurrentUserId currentUserId, HybridCache hybridCache,
            IHttpContextAccessor httpContextAccessor, ITenantRepository tenantRepository, IUnitOfWork unitOfWork)
        {
            _courseRepository = courseRepository;
            _tenantMemberRepository = tenantMemberRepository;
            _currentUserId = currentUserId;
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _tenantRepository = tenantRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<OneOf<SuccessDto, Error>> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var userId = _currentUserId.GetUserId();
            var isPermitted = await _tenantMemberRepository.IsPermittedMember(userId, PermissionConstants.DELETE_COURSES, cancellationToken);
            if (!isPermitted)
            {
                return MemberErrors.NotAllowed;
            }
            var course = await _courseRepository.GetCourseByIdAsync(request.CourseId, subDomain!, cancellationToken);
            if (course == null)
            {
                return CourseErrors.CourseNotFound;
            }
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                await _courseRepository.RemoveCourseAsync(course, cancellationToken);
                await _tenantRepository.DecreasePlanFeatureUsageByKeyAsync(subDomain!, FeatureConstants.COURSE_LIMIT, cancellationToken);
                await _hybridCache.RemoveByTagAsync(tags: new[] { $"{CacheKeysConstants.AllCoursesKey}_{subDomain}" }, cancellationToken);
                await _hybridCache.RemoveByTagAsync(tags: new[] { $"{CacheKeysConstants.AllCoursesKey}_{request.CourseId}" }, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return new SuccessDto
                {
                    Id = course.Id.ToString(),
                    Message = $"{nameof(Course)} {SuccessConstants.ItemDeleted}"
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
