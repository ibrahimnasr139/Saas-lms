namespace Application.Features.Students.Dtos
{
    public sealed class StudentStatsDto
    {
        public int TotalStudyHours { get; set; }
        public int AverageScore { get; set; }
        public int Rank { get; set; }
        public int TotalStudents { get; set; }
    }
}