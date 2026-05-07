using Domain.Enums;

namespace Application.Features.StudentLessons.Dtos
{
    public sealed class AiChatMessage
    {
        public int Id { get; set; }
        public RoleType Role { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}