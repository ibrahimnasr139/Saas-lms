using Application.Features.Discussions.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Discussions.Queries.GetDiscussionReplies
{
    internal sealed class GetDiscussionRepliesQueryHandler : IRequestHandler<GetDiscussionRepliesQuery, OneOf<List<DiscussionReplyDto>, Error>>
    {
        private readonly IDiscussionRepository _discussionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetDiscussionRepliesQueryHandler(IDiscussionRepository discussionRepository, IHttpContextAccessor httpContextAccessor)
        {
            _discussionRepository = discussionRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<OneOf<List<DiscussionReplyDto>, Error>> Handle(GetDiscussionRepliesQuery request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var thread = await _discussionRepository.GetThreadTenantAsync(request.ThreadId, subDomain!, cancellationToken);
            if (thread is null)
                return DiscussionErrors.DiscussionThreadNotFound;

            return await _discussionRepository.GetDiscussionReplyAsync(request.ThreadId, cancellationToken);
        }
    }
}