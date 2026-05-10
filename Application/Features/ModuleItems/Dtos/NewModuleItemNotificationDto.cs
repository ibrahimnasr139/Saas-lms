using Domain.Enums;

namespace Application.Features.ModuleItems.Dtos
{
    public sealed class NewModuleItemNotificationDto
    {
        public string StudentEmail { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string ItemTitle { get; set; } = string.Empty;
        public string CourseTitle { get; set; } = string.Empty;
        public ModuleItemType ModuleItemType { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}