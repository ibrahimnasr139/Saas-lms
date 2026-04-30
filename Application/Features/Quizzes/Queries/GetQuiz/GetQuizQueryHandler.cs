using Application.Features.ModuleItems.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Quizzes.Queries.GetQuiz
{
    internal sealed class GetQuizQueryHandler : IRequestHandler<GetQuizQuery, OneOf<QuizDto, Error>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IModuleItemRepository _moduleItemRepository;
        public GetQuizQueryHandler(IHttpContextAccessor httpContextAccessor, IModuleItemRepository moduleItemRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _moduleItemRepository = moduleItemRepository;
        }
        public async Task<OneOf<QuizDto, Error>> Handle(GetQuizQuery request, CancellationToken cancellationToken)
        {
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var quiz = await _moduleItemRepository.GetQuizWithQuestions(request.ItemId, request.ModuleId, request.CourseId, subdomain!, cancellationToken);
            if (quiz is null)
                return ModuleItemErrors.ModuleItemNotFound;
            return quiz;
        }
    }
}