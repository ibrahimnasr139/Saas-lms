using Microsoft.AspNetCore.Http;

namespace Application.Features.Questions.Commands.UpdateQuizQuestion
{
    internal sealed class UpdateQuizQuestionCommandHandler : IRequestHandler<UpdateQuizQuestionCommand, OneOf<bool, Error>>
    {
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IQuizRepository _quizRepository;
        private readonly IMapper _mapper;
        private readonly IQuestionRepository _questionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateQuizQuestionCommandHandler(ITenantMemberRepository tenantMemberRepository, ICurrentUserId currentUserId,
            ISubscriptionRepository subscriptionRepository, IHttpContextAccessor httpContextAccessor, ICourseRepository courseRepository,
            IQuizRepository quizRepository, IMapper mapper, IQuestionRepository questionRepository, IUnitOfWork unitOfWork)
        {
            _tenantMemberRepository = tenantMemberRepository;
            _currentUserId = currentUserId;
            _subscriptionRepository = subscriptionRepository;
            _httpContextAccessor = httpContextAccessor;
            _courseRepository = courseRepository;
            _quizRepository = quizRepository;
            _questionRepository = questionRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<OneOf<bool, Error>> Handle(UpdateQuizQuestionCommand request, CancellationToken cancellationToken)
        {

            var userId = _currentUserId.GetUserId();
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var isPermitted = await _tenantMemberRepository.IsPermittedMember(userId, PermissionConstants.MANAGE_MODULE_ITEMS, cancellationToken);
            if (!isPermitted)
                return MemberErrors.NotAllowed;

            var isSubscribed = await _subscriptionRepository.HasActiveSubscriptionByTenantDomain(subdomain!, cancellationToken);
            if (!isSubscribed)
                return TenantErrors.NotSubscribed;

            var quizQuestion = await _quizRepository.GetQuizQuestion(request.ItemId, request.QuizQuestionId, subdomain!, cancellationToken);
            if (quizQuestion is null)
                return QuestionErrors.QuestionNotFound;

            var isFound = await _questionRepository.IsFoundCategory(request.Category, cancellationToken);
            if (!isFound)
                return QuestionErrors.CategoryNotFound;

            _mapper.Map(request, quizQuestion);
            await _unitOfWork.SaveAsync(cancellationToken);
            return true;
        }
    }
}
