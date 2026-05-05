using Domain.Enums;

namespace Application.Features.StudentLessons.Dtos
{
    public sealed class StudentLessonItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ModuleItemType Type { get; set; }
        public ModuleItemStatus Status { get; set; }
        public bool IsCompleted { get; set; }
        public ContentDto Content { get; set; } = new();
        public List<Resource> Resources { get; set; } = [];
    }
}