using Application.Features.Public.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Public.Queries.GetCourseDetails
{
    internal sealed class GetCourseDetailsQueryHandler : IRequestHandler<GetCourseDetailsQuery, OneOf<WebsiteCourseDetailsDto, Error>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HybridCache _hybridCache;

        public GetCourseDetailsQueryHandler(ICourseRepository courseRepository, IHttpContextAccessor httpContextAccessor,
            HybridCache hybridCache)
        {
            _courseRepository = courseRepository;
            _httpContextAccessor = httpContextAccessor;
            _hybridCache = hybridCache;
        }
        public async Task<OneOf<WebsiteCourseDetailsDto, Error>> Handle(GetCourseDetailsQuery request, CancellationToken cancellationToken)
        {
            var sessionId = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SessionId];
            var cachedSessionKey = $"{CacheKeysConstants.SessionKey}_{sessionId}";
            var session = await _hybridCache.GetOrCreateAsync<UserSession?>(
                cachedSessionKey,
                _ => ValueTask.FromResult<UserSession?>(null),
                cancellationToken: cancellationToken
            );
            string subDomain = string.Empty;
            var httpRequest = _httpContextAccessor.HttpContext!.Request;
            var origin = httpRequest.Headers["Origin"].ToString();
            if (!string.IsNullOrEmpty(origin) && Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                subDomain = uri.Host.Split('.')[0];
            else
                subDomain = httpRequest.Host.Host.Split(".")[0];

            var websiteCourseDetails = await _courseRepository.GetWebsiteCourseDetailsAsync(request.CourseId, subDomain, session?.UserId, cancellationToken);
            if (websiteCourseDetails is null)
                return CourseErrors.CourseNotFound;

            return websiteCourseDetails;
        }
    }
}
