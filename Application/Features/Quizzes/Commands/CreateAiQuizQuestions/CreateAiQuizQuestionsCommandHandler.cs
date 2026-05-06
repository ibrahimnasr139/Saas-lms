using Application.Contracts.Externals;
using Application.Features.Quizzes.Dtos;
using Infrastructure.Common.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Application.Features.Quizzes.Commands.CreateAiQuizQuestions
{
    internal sealed class CreateAiQuizQuestionsCommandHandler : IRequestHandler<CreateAiQuizQuestionsCommand, OneOf<bool, Error>>
    {
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IExternalService _externalService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IQuizRepository _quizRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly AiOptions _options;
        public CreateAiQuizQuestionsCommandHandler(ITenantMemberRepository tenantMemberRepository, ICurrentUserId currentUserId,
            ISubscriptionRepository subscriptionRepository, IHttpContextAccessor httpContextAccessor, IExternalService externalService,
            IQuizRepository quizRepository, IQuestionRepository questionRepository, ITenantRepository tenantRepository, IUnitOfWork unitOfWork, IMapper mapper, IOptions<AiOptions> options)
        {
            _tenantMemberRepository = tenantMemberRepository;
            _currentUserId = currentUserId;
            _subscriptionRepository = subscriptionRepository;
            _httpContextAccessor = httpContextAccessor;
            _externalService = externalService;
            _quizRepository = quizRepository;
            _questionRepository = questionRepository;
            _tenantRepository = tenantRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _options = options.Value;
        }
        public async Task<OneOf<bool, Error>> Handle(CreateAiQuizQuestionsCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var isPermitted = await _tenantMemberRepository.IsPermittedMember(userId, PermissionConstants.MANAGE_QUIZZES, cancellationToken);
            if (!isPermitted)
            {
                return MemberErrors.NotAllowed;
            }
            var isSubscribed = await _subscriptionRepository.HasActiveSubscriptionByTenantDomain(subdomain!, cancellationToken);
            if (!isSubscribed)
            {
                return TenantErrors.NotSubscribed;
            }
            var metadata = await _quizRepository.GetQuizMetadata(request.ItemId, request.CourseId, request.ModuleId, subdomain!, cancellationToken);
            if (metadata is null)
            {
                return ModuleItemErrors.ModuleItemNotFound;
            }
            var payload = new AiPayload(request.Prompt, metadata, request.Difficulty, request.Type, request.QuestionsNumber);
            var endpoint = _options.QuestionEndPoint;
            var result = await _externalService.CallExternalServiceAsync<AiPayload, IEnumerable<AiResponse>>(endpoint, payload, cancellationToken);
            if (result is null)
            {
                throw new Exception();
            }
            var category = await _questionRepository.GetQuestionCategory(TenantMemberConstants.GeneralQuestionCategory, subdomain!, cancellationToken);
            var tenantId = await _tenantRepository.GetTenantIdAsync(subdomain!, cancellationToken);
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var quizQuestions = _mapper.Map<List<QuizQuestion>>(result);
                foreach (var quizQuestion in quizQuestions)
                {
                    quizQuestion.QuizId = request.ItemId;
                    quizQuestion.Question.QuestionCategoryId = category!.Id;
                    quizQuestion.Question.TenantId = tenantId;
                }
                await _questionRepository.AddQuestionsToQuiz(quizQuestions, cancellationToken);
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
