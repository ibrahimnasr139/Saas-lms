namespace Application.Features.StudyTools.Dtos
{
    public sealed class AttemptQuizDto
    {
        public int AttemptId { get; set; }
        public int? Score { get; set; }
        public bool? Passed { get; set; }
    }
}