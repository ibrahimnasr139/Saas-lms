using Microsoft.AspNetCore.Http;

namespace Application.Features.Questions.Commands.DeleteQuizQuestion
{
    internal sealed class DeleteQuizQuestionCommandHandler : IRequestHandler<DeleteQuizQuestionCommand, OneOf<SuccessDto, Error>>
    {
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IQuizRepository _quizRepository;

        public DeleteQuizQuestionCommandHandler(ITenantMemberRepository tenantMemberRepository, ICurrentUserId currentUserId,
            ISubscriptionRepository subscriptionRepository, IHttpContextAccessor httpContextAccessor, IQuizRepository quizRepository)
        {
            _tenantMemberRepository = tenantMemberRepository;
            _currentUserId = currentUserId;
            _subscriptionRepository = subscriptionRepository;
            _httpContextAccessor = httpContextAccessor;
            _quizRepository = quizRepository;
        }
        public async Task<OneOf<SuccessDto, Error>> Handle(DeleteQuizQuestionCommand request, CancellationToken cancellationToken)
        {

            var userId = _currentUserId.GetUserId();
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var isPermitted = await _tenantMemberRepository.IsPermittedMember(userId, PermissionConstants.MANAGE_MODULE_ITEMS, cancellationToken);

            if (!isPermitted)
                return MemberErrors.NotAllowed;

            var isSubscribed = await _subscriptionRepository.HasActiveSubscriptionByTenantDomain(subdomain!, cancellationToken);
            if (!isSubscribed)
                return TenantErrors.NotSubscribed;

            var quizQuestion = await _quizRepository.GetQuizQuestion(request.ItemId, request.QuestionId, subdomain!, cancellationToken);
            if (quizQuestion is null)
                return QuestionErrors.QuestionNotFound;

            await _quizRepository.RemoveQuizQuestion(quizQuestion, cancellationToken);
            return new SuccessDto { Message = $"{nameof(Question)} {SuccessConstants.ItemDeleted}" };
        }
    }
}
