using Application.Features.Discussions.Dtos;

namespace Application.Features.Discussions.Commands.UpdateDiscussionReply
{
    public sealed record UpdateDiscussionReplyCommand(int ThreadId, int ReplyId, string Body)
        : IRequest<OneOf<DiscussionResponse, Error>>;
}