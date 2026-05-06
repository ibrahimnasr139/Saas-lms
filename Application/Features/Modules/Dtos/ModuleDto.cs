namespace Application.Features.Modules.Dtos
{
    public sealed class ModuleDto
    {
        public string? Description { get; set; }
        public string Title { get; set; } = null!;
        public int Order { get; set; }
        public int TotalItems { get; set; }
        public int TotalLessons { get; set; }
        public int TotalQuizzes { get; set; }
        public int TotalAssignments { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public IEnumerable<ModuleItemDto> Items { get; set; } = null!;
    }
}
