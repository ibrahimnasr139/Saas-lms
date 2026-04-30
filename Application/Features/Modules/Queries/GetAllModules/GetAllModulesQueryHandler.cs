using Application.Features.Modules.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Modules.Queries.GetAllModules
{
    internal sealed class GetAllModulesQueryHandler : IRequestHandler<GetAllModulesQuery, OneOf<IEnumerable<AllModulesDto>, Error>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IModuleRepository _moduleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HybridCache _hybridCache;
        public GetAllModulesQueryHandler(ICourseRepository courseRepository, IModuleRepository moduleRepository, IHttpContextAccessor httpContextAccessor, HybridCache hybridCache)
        {
            _courseRepository = courseRepository;
            _moduleRepository = moduleRepository;
            _httpContextAccessor = httpContextAccessor;
            _hybridCache = hybridCache;
        }
        public async Task<OneOf<IEnumerable<AllModulesDto>, Error>> Handle(GetAllModulesQuery request, CancellationToken cancellationToken)
        {
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var course = await _courseRepository.GetCourseByIdAsync(request.CourseId, subdomain!, cancellationToken);
            if (course is null)
                return CourseErrors.CourseNotFound;

            var cacheKey = $"{CacheKeysConstants.CourseModuleKey}-{request.CourseId}";
            return await _hybridCache.GetOrCreateAsync(
                cacheKey,
                async cacheEntry =>
                {
                    return await _moduleRepository.GetAllModulesAsync(request.CourseId, cancellationToken);
                },
                new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(30)
                },
                tags: new[] { $"{CacheKeysConstants.AllCoursesKey}_{request.CourseId}" },
                cancellationToken: cancellationToken
            );
        }
    }
}
