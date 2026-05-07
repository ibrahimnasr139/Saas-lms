using Application.Features.Attempts.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Attempts.Queries.GetAttempt
{
    internal sealed class GetAttemptQueryHandler : IRequestHandler<GetAttemptQuery, OneOf<AttemptResponse, Error>>
    {
        private readonly IAttemptRepository _attemptRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetAttemptQueryHandler(IAttemptRepository attemptRepository, IHttpContextAccessor httpContextAccessor)
        {
            _attemptRepository = attemptRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<OneOf<AttemptResponse, Error>> Handle(GetAttemptQuery request, CancellationToken cancellationToken)
        {
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var attempt = await _attemptRepository.GetAttemptResponseByIdAsync(request.AttemptId, request.QuizId, subdomain!, cancellationToken);
            if (attempt is null)
                return AttemptErrors.AttemptNotFound;
            return attempt;
        }
    }
}
