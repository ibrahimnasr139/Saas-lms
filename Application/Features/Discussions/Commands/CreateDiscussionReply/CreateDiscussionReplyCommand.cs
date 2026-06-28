using Application.Features.StudentLessons.Dtos;

namespace Application.Features.Discussions.Commands.CreateDiscussionReply
{
    public sealed record CreateDiscussionReplyCommand(int ThreadId, int CourseId, int ItemId, string Content)
        : IRequest<OneOf<StudentLessonResponse, Error>>;
}