namespace Application.Features.Students.Dtos
{
    public sealed class ProfileDetailsDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Avatar { get; set; }
        public string? Bio { get; set; }
        public string? Grade { get; set; }
        public string? Semester { get; set; }
        public List<StudentSubjectProfileDto> Subjects { get; set; } = new List<StudentSubjectProfileDto>();
        public StudentGamificationDto Gamification { get; set; } = new StudentGamificationDto();
        public ProfileStreakDto Streak { get; set; } = new ProfileStreakDto();
        public StudentStatsDto Stats { get; set; } = new StudentStatsDto();
        public DateTime JoinedAt { get; set; }
    }
}