namespace Application.Features.Modules.Dtos
{
    public sealed class ItemMetaDataDto
    {
        public string? Description { get; set; }
        public string Title { get; set; } = null!;
        public int? Views { get; set; }
        public int? QuestionsCount { get; set; }
        public double? PassingScore { get; set; }
        public double? AverageScore { get; set; }
        public int? Attempts { get; set; }
        public int? Submissions { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
