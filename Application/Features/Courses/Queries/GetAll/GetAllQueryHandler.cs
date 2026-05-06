using Application.Features.Courses.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Courses.Queries.GetAll
{
    internal sealed class GetAllQueryHandler : IRequestHandler<GetAllQuery, AllCoursesDto>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetAllQueryHandler(ICourseRepository courseRepository, HybridCache hybridCache, IHttpContextAccessor httpContextAccessor)
        {
            _courseRepository = courseRepository;
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AllCoursesDto> Handle(GetAllQuery request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var cacheKey = $"{CacheKeysConstants.AllCoursesKey}_{request.Q}_{request.GradeId}_{request.SubjectId}_{request.SortBy}_{request.SortOrder}_{request.LastSortValue}_{request.Status}_{request.Cursor}_{subDomain}";
            var courses = await _hybridCache.GetOrCreateAsync(
                cacheKey,
                async cacheEntry =>
                {
                    return await _courseRepository.GetAllCoursesAsync(
                        subDomain: subDomain!,
                        q: request.Q,
                        gradeId: request.GradeId,
                        subjectId: request.SubjectId,
                        sortBy: request.SortBy,
                        sortOrder: request.SortOrder,
                        status: request.Status,
                        cursor: request.Cursor,
                        lastSortValue: request.LastSortValue,
                        cancellationToken: cancellationToken
                    );
                },
                new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(30)
                },
                tags: new[] { $"{CacheKeysConstants.AllCoursesKey}_{subDomain}" },
                cancellationToken: cancellationToken
            );
            return courses;
        }
    }
}
