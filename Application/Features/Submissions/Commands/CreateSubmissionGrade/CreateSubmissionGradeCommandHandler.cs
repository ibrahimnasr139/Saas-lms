using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Submissions.Commands.CreateSubmissionGrade
{
    internal sealed class CreateSubmissionGradeCommandHandler : IRequestHandler<CreateSubmissionGradeCommand, OneOf<bool, Error>>
    {
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantRepository _tenantRepository;
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IStudentGradeRepository _studentGradeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateSubmissionGradeCommandHandler(ITenantMemberRepository tenantMemberRepository, ICurrentUserId currentUserId,
            ISubscriptionRepository subscriptionRepository, IHttpContextAccessor httpContextAccessor, ITenantRepository tenantRepository,
            ISubmissionRepository submissionRepository, IStudentGradeRepository studentGradeRepository, IUnitOfWork unitOfWork)
        {
            _tenantMemberRepository = tenantMemberRepository;
            _currentUserId = currentUserId;
            _subscriptionRepository = subscriptionRepository;
            _httpContextAccessor = httpContextAccessor;
            _tenantRepository = tenantRepository;
            _submissionRepository = submissionRepository;
            _studentGradeRepository = studentGradeRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<bool, Error>> Handle(CreateSubmissionGradeCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var tenantId = await _tenantRepository.GetTenantIdAsync(subdomain!, cancellationToken);
            var isPermitted = await _tenantMemberRepository.IsPermittedMember(userId, PermissionConstants.MANAGE_SUBMISSIONS, cancellationToken);
            if (!isPermitted)
                return MemberErrors.NotAllowed;

            var isSubscribed = await _subscriptionRepository.HasActiveSubscriptionByTenantDomain(subdomain!, cancellationToken);
            if (!isSubscribed)
                return TenantErrors.NotSubscribed;

            var submission = await _submissionRepository.GetSubmissionAsync(request.SubmissionId, request.ItemId, cancellationToken);
            if (submission is null)
                return SubmissionErrors.SubmissionNotFound;

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                await _submissionRepository.GradeSubmission(request.SubmissionId, request.Grade, request.Feedback, cancellationToken);

                var studentGrade = await _studentGradeRepository.GetStudentGradeAsync(submission.StudentId, request.ItemId, cancellationToken);
                if (studentGrade is null)
                {
                    var newStudentGrade = new StudentGrade
                    {
                        Score = request.Grade,
                        TotalMarks = submission.Assignment.Marks,
                        Type = StudentGradeType.Assignment,
                        GraderId = await _tenantMemberRepository.GetTenantmemberIdAsync(tenantId, cancellationToken),
                        StudentId = submission.StudentId,
                        TypeId = request.ItemId,
                        TenantId = tenantId
                    };
                    await _studentGradeRepository.CreateStudentGradeAsync(newStudentGrade, cancellationToken);
                }
                else
                    studentGrade.Score = request.Grade;
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
            return true;
        }
    }
}