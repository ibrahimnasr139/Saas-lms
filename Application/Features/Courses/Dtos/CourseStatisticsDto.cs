namespace Application.Features.Courses.Dtos
{
    public sealed class CourseStatisticsDto
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int TotalModules { get; set; }
        public int TotalLessons { get; set; }
        public int TotalAssignments { get; set; }
        public int TotalQuizzes { get; set; }
        public int TotalStudents { get; set; }
    }
}
