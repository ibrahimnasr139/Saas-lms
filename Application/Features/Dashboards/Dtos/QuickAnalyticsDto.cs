namespace Application.Features.Dashboards.Dtos
{
    public sealed class QuickAnalyticsDto
    {
        public int TotalCourses { get; set; }
        public int TotalLessons { get; set; }
        public int NewMessages { get; set; }
        public int CompletionRate { get; set; }
    }
}