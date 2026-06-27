namespace Application.Features.Students.Dtos
{
    public sealed class StudentStatsDto
    {
        public double TotalStudyHours { get; set; }
        public double AverageScore { get; set; }
        public int Rank { get; set; }
        public int TotalStudents { get; set; }
    }
}