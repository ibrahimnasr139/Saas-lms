using Microsoft.AspNetCore.Http;

namespace Application.Features.Attempts.Commands.CreateAttemptManualGrading
{
    internal sealed class CreateAttemptManualGradeCommandHandler : IRequestHandler<CreateAttemptManualGradeCommand, OneOf<bool, Error>>
    {
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IAttemptRepository _attemptRepository;
        private readonly IAnswerRepository _answerRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        public CreateAttemptManualGradeCommandHandler(ITenantMemberRepository tenantMemberRepository, ICurrentUserId currentUserId, ISubscriptionRepository subscriptionRepository,
            IAttemptRepository attemptRepository, IAnswerRepository answerRepository, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _tenantMemberRepository = tenantMemberRepository;
            _currentUserId = currentUserId;
            _subscriptionRepository = subscriptionRepository;
            _attemptRepository = attemptRepository;
            _answerRepository = answerRepository;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<bool, Error>> Handle(CreateAttemptManualGradeCommand request, CancellationToken cancellationToken)
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
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                await _answerRepository.UpdateTeacherScore(request.AttemptId, request.Questions, cancellationToken);
                attempt.Score = request.OverallScore;
                attempt.GradingStatus = request.Status;
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
