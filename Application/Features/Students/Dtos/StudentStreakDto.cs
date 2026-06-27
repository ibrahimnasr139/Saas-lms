namespace Application.Features.Students.Dtos
{
    public sealed class StudentStreakDto
    {
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public DateTime? LastActivityAt { get; set; }
    }
}