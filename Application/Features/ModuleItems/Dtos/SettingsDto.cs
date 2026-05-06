using Domain.Enums;

namespace Application.Features.ModuleItems.Dtos
{
    public sealed class SettingsDto
    {
        public int Id { get; set; }
        public ModuleItemType Type { get; set; }
        public CourseStatus Status { get; set; }
        public bool AllowDiscussions { get; set; }
        public IEnumerable<ConditionDto> Conditions { get; set; } = [];
    }
}
