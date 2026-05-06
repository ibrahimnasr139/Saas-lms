using Application.Features.Attempts.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Quizzes.Queries.GetAttempts
{
    internal sealed class GetAttemptsQueryHandler : IRequestHandler<GetAttemptsQuery, OneOf<IEnumerable<AttemptDto>, Error>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IQuizRepository _quizRepository;
        private readonly IModuleItemRepository _moduleItemRepository;
        public GetAttemptsQueryHandler(IHttpContextAccessor httpContextAccessor, IQuizRepository quizRepository, IModuleItemRepository moduleItemRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _quizRepository = quizRepository;
            _moduleItemRepository = moduleItemRepository;
        }
        public async Task<OneOf<IEnumerable<AttemptDto>, Error>> Handle(GetAttemptsQuery request, CancellationToken cancellationToken)
        {
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var quiz = await _moduleItemRepository.GetQuizAsync(request.ItemId, request.ModuleId, request.CourseId, subdomain!, cancellationToken);
            if (quiz is null)
            {
                return ModuleItemErrors.ModuleItemNotFound;
            }
            return await _quizRepository.GetAttempts(request.CourseId, request.ItemId, cancellationToken);
        }
    }
}
