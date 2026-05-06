using Application.Features.Courses.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Courses.Queries.GetCourseStatistics
{
    internal sealed class GetCourseStatisticsQueryHandler : IRequestHandler<GetCourseStatisticsQuery, OneOf<CourseStatisticsDto, Error>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HybridCache _hybridCache;
        public GetCourseStatisticsQueryHandler(ICourseRepository courseRepository, IHttpContextAccessor httpContextAccessor, HybridCache hybridCache)
        {
            _courseRepository = courseRepository;
            _httpContextAccessor = httpContextAccessor;
            _hybridCache = hybridCache;
        }
        public async Task<OneOf<CourseStatisticsDto, Error>> Handle(GetCourseStatisticsQuery request, CancellationToken cancellationToken)
        {
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var cacheKey = $"{CacheKeysConstants.CourseStatisticsKey}-{request.CourseId}";
            var course = await _hybridCache.GetOrCreateAsync(
                cacheKey,
                async cacheEntry =>
                {
                    return await _courseRepository.GetCourseStatisticsByIdAsync(request.CourseId, subdomain!, cancellationToken);
                },
                new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(30)
                },
                tags: new[] { $"{CacheKeysConstants.AllCoursesKey}_{request.CourseId}" },
                cancellationToken: cancellationToken
            );
            if (course is null)
            {
                return CourseErrors.CourseNotFound;
            }
            return course;
        }
    }
}
