using Microsoft.AspNetCore.Http;

namespace Application.Features.Questions.Commands.UpdateQuestion
{
    internal sealed class UpdateQuestionCommandHandler : IRequestHandler<UpdateQuestionCommand, OneOf<SuccessDto, Error>>
    {

        private readonly IQuestionRepository _questionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantRepository _tenantRepository;
        private readonly IMapper _mapper;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateQuestionCommandHandler(IQuestionRepository questionRepository, IHttpContextAccessor httpContextAccessor,
            ITenantRepository tenantRepository, IMapper mapper, ISubscriptionRepository subscriptionRepository,
            IUnitOfWork unitOfWork)
        {
            _questionRepository = questionRepository;
            _httpContextAccessor = httpContextAccessor;
            _tenantRepository = tenantRepository;
            _mapper = mapper;
            _subscriptionRepository = subscriptionRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<SuccessDto, Error>> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var isSubscribed = await _subscriptionRepository.HasActiveSubscriptionByTenantDomain(subDomain!, cancellationToken);
            if (!isSubscribed)
            {
                return TenantErrors.NotSubscribed;
            }
            var isFoundCategory = await _questionRepository.IsFoundCategory(request.Category, cancellationToken);
            if (!isFoundCategory)
            {
                return QuestionErrors.CategoryNotFound;
            }
            var question = await _questionRepository.GetQuestion(request.QuestionId, subDomain!, cancellationToken);
            if (question == null)
            {
                return QuestionErrors.QuestionNotFound;
            }
            _mapper.Map(request, question);
            await _unitOfWork.SaveAsync(cancellationToken);
            return new SuccessDto
            {
                Id = question.Id.ToString(),
                Message = $"{nameof(Module)} {SuccessConstants.ItemUpdated}"
            };
        }
    }
}
