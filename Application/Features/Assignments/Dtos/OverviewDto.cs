namespace Application.Features.Assignments.Dtos
{
    public sealed class OverviewDto
    {
        public int TotalSubmissions { get; set; }
        public int TotalStudents { get; set; }
        public double? AverageScore { get; set; }
        public double? HighestScore { get; set; }
        public double? LowestScore { get; set; }
    }
}
