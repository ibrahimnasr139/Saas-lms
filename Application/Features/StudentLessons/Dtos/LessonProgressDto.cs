namespace Application.Features.StudentLessons.Dtos
{
    public sealed class LessonProgressDto
    {
        public int ItemId { get; set; }
        public int CourseId { get; set; }
        public string? VideoId { get; set; }
        public int WatchedSeconds { get; set; }
        public int DurationSeconds { get; set; }
        public double CompletionPercentage { get; set; }
        public bool IsCompleted { get; set; }
        public int LastPositionSeconds { get; set; }
        public int ViewsCount { get; set; }
        public DateTime? FirstViewedAt { get; set; }
        public DateTime? LastViewedAt { get; set; }
    }
}