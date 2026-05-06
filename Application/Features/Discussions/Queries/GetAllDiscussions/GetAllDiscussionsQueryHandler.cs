using Application.Features.Discussions.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Discussions.Queries.GetAllDiscussions
{
    internal sealed class GetAllDiscussionsQueryHandler : IRequestHandler<GetDiscussionsQuery, AllDiscussionsDto>
    {
        private readonly IDiscussionRepository _discussionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUserId _currentUserId;

        public GetAllDiscussionsQueryHandler(IDiscussionRepository discussionRepository, IHttpContextAccessor httpContextAccessor,
            ICurrentUserId currentUserId)
        {
            _discussionRepository = discussionRepository;
            _httpContextAccessor = httpContextAccessor;
            _currentUserId = currentUserId;
        }
        public async Task<AllDiscussionsDto> Handle(GetDiscussionsQuery request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var currentUserId = _currentUserId.GetUserId();
            return await _discussionRepository.GetAllDiscussionsAsync(
                subDomain: subDomain!,
                currentUser: currentUserId,
                Q: request.Q,
                CourseId: request.CourseId,
                Type: request.Type,
                Cursor: request.Cursor,
                Limit: request.Limit,
                cancellationToken: cancellationToken
            );
        }
    }
}