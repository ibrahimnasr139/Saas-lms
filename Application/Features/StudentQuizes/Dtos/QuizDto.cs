namespace Application.Features.StudentQuizes.Dtos
{
    public sealed class QuizDto
    {
        public int Id { get; set; }
        public int TotalMarks { get; set; }
        public bool HasStarted { get; set; }
        public int Duration { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}