using Application.Features.Discussions.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Discussions.Commands.DeleteDiscussionThread
{
    internal sealed class DeleteDiscussionThreadCommandHandler : IRequestHandler<DeleteDiscussionThreadCommand, OneOf<DiscussionResponse, Error>>
    {
        private readonly IDiscussionRepository _discussionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteDiscussionThreadCommandHandler(IDiscussionRepository discussionRepository, IHttpContextAccessor httpContextAccessor)
        {
            _discussionRepository = discussionRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<OneOf<DiscussionResponse, Error>> Handle(DeleteDiscussionThreadCommand request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var isDeleted = await _discussionRepository.DeleteDiscussionThreadAsync(request.ThreadId, subDomain!, cancellationToken);
            if (!isDeleted)
                return DiscussionErrors.DiscussionThreadNotFound;

            return new DiscussionResponse { Message = MessagesConstants.DiscussionThreadDeleted };
        }
    }
}