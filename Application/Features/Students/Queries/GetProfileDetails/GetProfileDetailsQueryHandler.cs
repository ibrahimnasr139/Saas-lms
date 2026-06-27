using Application.Features.Students.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Students.Queries.GetProfileDetails
{
    internal sealed class GetProfileDetailsQueryHandler : IRequestHandler<GetProfileDetailsQuery, OneOf<ProfileDetailsDto, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IStudentRepository _studentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetProfileDetailsQueryHandler(HybridCache hybridCache, IStudentRepository studentUserRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _hybridCache = hybridCache;
            _studentRepository = studentUserRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<OneOf<ProfileDetailsDto, Error>> Handle(GetProfileDetailsQuery request, CancellationToken cancellationToken)
        {
            var sessionId = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SessionId];
            var cachedSessionKey = $"{CacheKeysConstants.SessionKey}_{sessionId}";
            var session = await _hybridCache.GetOrCreateAsync<UserSession?>(
                cachedSessionKey,
                _ => ValueTask.FromResult<UserSession?>(null),
                cancellationToken: cancellationToken
            );
            if (session is null)
                return UserErrors.Unauthorized;
            throw new NotImplementedException();
        }
    }
}