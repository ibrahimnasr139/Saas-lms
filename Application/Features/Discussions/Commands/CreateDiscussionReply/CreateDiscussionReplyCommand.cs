using Application.Features.StudentLessons.Dtos;

namespace Application.Features.Discussions.Commands.CreateDiscussionReply
{
    public sealed record CreateDiscussionReplyCommand(int ThreadId, string Content) : IRequest<OneOf<StudentLessonResponse, Error>>;
}