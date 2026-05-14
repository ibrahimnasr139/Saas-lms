using Domain.Enums;
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
        private readonly IStudentGradeRepository _studentGradeRepository;
        private readonly ITenantRepository _tenantRepository;

        public CreateAttemptManualGradeCommandHandler(ITenantMemberRepository tenantMemberRepository, ICurrentUserId currentUserId, ISubscriptionRepository subscriptionRepository,
            IAttemptRepository attemptRepository, IAnswerRepository answerRepository, IHttpContextAccessor httpContextAccessor, 
            IUnitOfWork unitOfWork, IStudentGradeRepository studentGradeRepository, ITenantRepository tenantRepository)
        {
            _tenantMemberRepository = tenantMemberRepository;
            _currentUserId = currentUserId;
            _subscriptionRepository = subscriptionRepository;
            _attemptRepository = attemptRepository;
            _answerRepository = answerRepository;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
            _studentGradeRepository = studentGradeRepository;
            _tenantRepository = tenantRepository;
        }
        public async Task<OneOf<bool, Error>> Handle(CreateAttemptManualGradeCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var isPermitted = await _tenantMemberRepository.IsPermittedMember(userId, PermissionConstants.MANAGE_ATTEMPTS, cancellationToken);
            if (!isPermitted)
                return MemberErrors.NotAllowed;
            
            var isSubscribed = await _subscriptionRepository.HasActiveSubscriptionByTenantDomain(subdomain!, cancellationToken);
            if (!isSubscribed)
                return TenantErrors.NotSubscribed;
            
            var attempt = await _attemptRepository.GetAttemptByIdAsync(request.AttemptId, request.QuizId, subdomain!, cancellationToken);
            if (attempt is null)
                return AttemptErrors.AttemptNotFound;
            
            var tenantId = await _tenantRepository.GetTenantIdAsync(subdomain!, cancellationToken);
            var graderId = await _tenantMemberRepository.GetTenantmemberIdAsync(tenantId, cancellationToken);
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                await _answerRepository.UpdateTeacherScore(request.AttemptId, request.Questions, cancellationToken);
                attempt.Score = request.OverallScore;
                attempt.GradingStatus = request.Status;

                var newStudentGrade = new StudentGrade
                {
                    Score = request.OverallScore,
                    TotalMarks = attempt.TotalMarks,
                    Type = StudentGradeType.Quiz,
                    GraderId = graderId,
                    StudentId = attempt.StudentId,
                    TypeId = request.QuizId,
                    TenantId = tenantId
                };
                await _studentGradeRepository.CreateStudentGradeAsync(newStudentGrade, cancellationToken);
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
