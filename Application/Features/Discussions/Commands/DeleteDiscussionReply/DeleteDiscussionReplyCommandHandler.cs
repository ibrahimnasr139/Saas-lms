using Application.Features.Discussions.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Discussions.Commands.DeleteDiscussionReply
{
    internal sealed class DeleteDiscussionReplyCommandHandler : IRequestHandler<DeleteDiscussionReplyCommand, OneOf<DiscussionResponse, Error>>
    {
        private readonly IDiscussionRepository _discussionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteDiscussionReplyCommandHandler(IDiscussionRepository discussionRepository, IHttpContextAccessor httpContextAccessor)
        {
            _discussionRepository = discussionRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<OneOf<DiscussionResponse, Error>> Handle(DeleteDiscussionReplyCommand request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var replyIsDeleted = await _discussionRepository.DeleteDiscussionThreadReplyAsync(request.ThreadId, request.ReplyId, subDomain!, cancellationToken);
            if (!replyIsDeleted)
                return DiscussionErrors.DiscussionReplyNotFound;
            return new DiscussionResponse { Message = MessagesConstants.DiscussionReplyDeleted };
        }
    }
}