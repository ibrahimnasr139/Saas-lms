namespace Application.Features.Students.Dtos
{
    public sealed class CurrentStudentDto
    {
        public int Id { get; set; }
        public string? Grade { get; set; }
        public string? InviteCode { get; set; }
        public ProfileDto Profile { get; set; } = new();
        public GamificationDto Gamification { get; set; } = new();
        public ProfileStreakDto Streak { get; set; } = new();
    }
}