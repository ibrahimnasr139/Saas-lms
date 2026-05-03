using Domain.Enums;

namespace Application.Features.StudentCourse.Dtos
{
    public sealed class ModuleItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public ModuleItemType Type { get; set; }
        public ModuleItemStatus Status { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Reason { get; set; }
    }
}