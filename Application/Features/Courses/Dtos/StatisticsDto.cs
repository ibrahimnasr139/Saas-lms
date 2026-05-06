namespace Application.Features.Courses.Dtos
{
    public sealed class StatisticsDto
    {
        public int TotalCourses { get; set; }
        public int ActiveCourses { get; set; }
        public int DraftCourses { get; set; }
        public int TotalStudentsEnrolled { get; set; }
        public double AverageCompletionRate { get; set; }
    }
}
