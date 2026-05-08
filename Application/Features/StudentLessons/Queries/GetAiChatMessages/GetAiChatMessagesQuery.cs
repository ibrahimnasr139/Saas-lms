using Application.Features.StudentLessons.Dtos;

namespace Application.Features.StudentLessons.Queries.GetAiChatMessages
{
    public sealed record GetAiChatMessagesQuery(int CourseId, int ItemId) : IRequest<OneOf<List<AiChatMessage>, Error>>;
}