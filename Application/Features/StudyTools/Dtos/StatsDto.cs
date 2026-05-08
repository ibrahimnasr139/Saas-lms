namespace Application.Features.StudyTools.Dtos
{
    public sealed class StatsDto
    {
        public int TotalCards { get; set; }
        public int KnownCards { get; set; }
        public int LearningCards { get; set; }
        public int NewCards { get; set; }
        public int Progress { get; set; }
    }
}