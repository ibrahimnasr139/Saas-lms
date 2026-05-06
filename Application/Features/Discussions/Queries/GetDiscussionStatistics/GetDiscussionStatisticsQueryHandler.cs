using Application.Features.Discussions.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Discussions.Queries.GetDiscussionStatistics
{
    internal sealed class GetDiscussionStatisticsQueryHandler : IRequestHandler<GetDiscussionStatisticsQuery, DiscussionStatisticsDto>
    {
        private readonly IDiscussionRepository _discussionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HybridCache _hybridCache;

        public GetDiscussionStatisticsQueryHandler(IDiscussionRepository discussionRepository, IHttpContextAccessor httpContextAccessor,
            HybridCache hybridCache)
        {
            _discussionRepository = discussionRepository;
            _httpContextAccessor = httpContextAccessor;
            _hybridCache = hybridCache;
        }

        public async Task<DiscussionStatisticsDto> Handle(GetDiscussionStatisticsQuery request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var cacheKey = $"{CacheKeysConstants.DiscussionStatistics}_{subDomain}";

            var result = await _hybridCache.GetOrCreateAsync(
                cacheKey,
                async cacheEntry =>
                {
                    return await _discussionRepository.GetDiscussionStatisticsAsync(subDomain!, cancellationToken);
                },
                new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromHours(2)
                },
                cancellationToken: cancellationToken
            );
            return result;
        }
    }
}