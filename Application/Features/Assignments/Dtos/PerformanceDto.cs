namespace Application.Features.Assignments.Dtos
{
    public sealed class PerformanceDto
    {
        public IEnumerable<SubmissionOverTime> SubmissionsOverTime { get; set; } = null!;
        public IEnumerable<GradeDistribution> GradesDistribution { get; set; } = null!;

    }
}
