using Domain.Enums;

namespace Domain.Entites
{
    public sealed class AiAssistantMessage
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public RoleType Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;
        public int LessonId { get; set; }
        public ModuleItem ModuleItem { get; set; } = null!;
    }
}