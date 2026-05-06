using Domain.Enums;

namespace Application.Features.Modules.Dtos
{
    public sealed class ModuleItemDto
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public ModuleItemType ItemType { get; set; }
        public CourseStatus Status { get; set; }
        public ItemMetaDataDto MetaData { get; set; } = null!;
    }
}
