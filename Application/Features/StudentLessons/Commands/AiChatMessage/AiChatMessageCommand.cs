using Application.Features.StudentLessons.Dtos;

namespace Application.Features.StudentLessons.Commands.AiChatMessage
{
    public sealed record AiChatMessageCommand(int CourseId, int ItemId, string Message) : IRequest<OneOf<AiChatMessageDto, Error>>;
}