using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Attempts.Commands.PublishAttempt
{
    internal sealed class PublishAttemptCommandHandler : IRequestHandler<PublishAttemptCommand, OneOf<SuccessDto, Error>>
    {
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IAttemptRepository _attemptRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        public PublishAttemptCommandHandler(ITenantMemberRepository tenantMemberRepository, ICurrentUserId currentUserId, ISubscriptionRepository subscriptionRepository,
            IAttemptRepository attemptRepository, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _tenantMemberRepository = tenantMemberRepository;
            _currentUserId = currentUserId;
            _subscriptionRepository = subscriptionRepository;
            _attemptRepository = attemptRepository;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<SuccessDto, Error>> Handle(PublishAttemptCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var isPermitted = await _tenantMemberRepository.IsPermittedMember(userId, PermissionConstants.MANAGE_ATTEMPTS, cancellationToken);
            if (!isPermitted)
            {
                return MemberErrors.NotAllowed;
            }
            var isSubscribed = await _subscriptionRepository.HasActiveSubscriptionByTenantDomain(subdomain!, cancellationToken);
            if (!isSubscribed)
            {
                return TenantErrors.NotSubscribed;
            }
            var attempt = await _attemptRepository.GetAttemptByIdAsync(request.AttemptId, request.QuizId, subdomain!, cancellationToken);
            if (attempt is null)
            {
                return AttemptErrors.AttemptNotFound;
            }
            if (attempt.GradingStatus == GradingStatus.Published)
            {
                return AttemptErrors.AttemptAlreadyPublished;
            }
            if (attempt.GradingStatus != GradingStatus.Graded)
            {
                return AttemptErrors.AttemptNotGraded;
            }
            attempt.GradingStatus = GradingStatus.Published;
            await _unitOfWork.SaveAsync(cancellationToken);
            return new SuccessDto { Message = "تم نشر هذه المحاولة بنجاح" };

        }
    }
}
