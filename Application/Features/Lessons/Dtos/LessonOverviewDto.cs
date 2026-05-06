namespace Application.Features.Lessons.Dtos
{
    public sealed class LessonOverviewDto
    {
        public int TotalViews { get; set; }
        public double CompletionRate { get; set; }
        public double AverageWatchTime { get; set; }
        public DateTime? PeakActivity { get; set; }
        public int TotalStudents { get; set; }
    }
}
