using Application.Features.StudentLessons.Dtos;

namespace Application.Features.StudentLessons.Queries.GetAiChatMessage
{
    public sealed record GetAiChatMessageQuery(int CourseId, int ItemId) : IRequest<OneOf<AiChatMessage, Error>>;
}