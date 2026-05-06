using Application.Features.Questions.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Questions.Queries.GetQuestion
{
    internal sealed class GetQuestionQueryHandler : IRequestHandler<GetQuestionQuery, OneOf<SingleQuestionDto, Error>>
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public GetQuestionQueryHandler(IQuestionRepository questionRepository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _questionRepository = questionRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        public async Task<OneOf<SingleQuestionDto, Error>> Handle(GetQuestionQuery request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var question = await _questionRepository.GetQuestion(request.QuestionId, subDomain!, cancellationToken);
            if (question is null)
            {
                return QuestionErrors.QuestionNotFound;
            }
            return _mapper.Map<SingleQuestionDto>(question);
        }
    }
}
