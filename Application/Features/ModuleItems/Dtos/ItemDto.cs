using Domain.Enums;

namespace Application.Features.ModuleItems.Dtos
{
    public sealed class ItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public CourseStatus Status { get; set; }
        public ModuleItemType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
