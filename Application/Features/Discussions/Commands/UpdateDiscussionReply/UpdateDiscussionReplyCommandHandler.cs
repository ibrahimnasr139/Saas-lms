using Application.Features.Discussions.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Discussions.Commands.UpdateDiscussionReply
{
    internal sealed class UpdateDiscussionReplyCommandHandler : IRequestHandler<UpdateDiscussionReplyCommand, OneOf<DiscussionResponse, Error>>
    {
        private readonly IDiscussionRepository _discussionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateDiscussionReplyCommandHandler(IDiscussionRepository discussionRepository, IHttpContextAccessor httpContextAccessor)
        {
            _discussionRepository = discussionRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<OneOf<DiscussionResponse, Error>> Handle(UpdateDiscussionReplyCommand request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var discussionReplyIsUpdated = await _discussionRepository.UpdateDiscussionReplyAsync(request.ThreadId, request.ReplyId, request.Body, subDomain!, cancellationToken);
            if (!discussionReplyIsUpdated)
                return DiscussionErrors.DiscussionReplyNotFound;
            return new DiscussionResponse { Message = MessagesConstants.DiscussionReplyUpdated };
        }
    }
}
