using Domain.Enums;

namespace Application.Features.ModuleItems.Dtos
{
    public sealed class ConditionDto
    {
        public int Id { get; set; }
        public ConditionType ConditionType { get; set; }
        public int? RequiredItemId { get; set; }
        public int? Value { get; set; }
        public ConditionEffect Effect { get; set; }
        public bool Enabled { get; set; }
        public string Message { get; set; } = null!;
    }
}