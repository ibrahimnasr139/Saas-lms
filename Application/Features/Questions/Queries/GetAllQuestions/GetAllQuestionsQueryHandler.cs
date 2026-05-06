using Application.Features.Questions.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Questions.Queries.GetAllQuestions
{
    internal sealed class GetAllQuestionsQueryHandler : IRequestHandler<GetAllQuestionsQuery, IEnumerable<AllQuestionsDto>>
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetAllQuestionsQueryHandler(IQuestionRepository questionRepository, IHttpContextAccessor httpContextAccessor)
        {
            _questionRepository = questionRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IEnumerable<AllQuestionsDto>> Handle(GetAllQuestionsQuery request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            return await _questionRepository.GetAllQuestions(subDomain!, cancellationToken);
        }
    }
}
