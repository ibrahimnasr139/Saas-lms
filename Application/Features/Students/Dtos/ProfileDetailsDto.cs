namespace Application.Features.Students.Dtos
{
    public sealed class ProfileDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string? Grade { get; set; }
        public string? Semester { get; set; }
        public List<StudentSubjectDto> Subjects { get; set; } = new List<StudentSubjectDto>();
        public StudentGamificationDto Gamification { get; set; } = new StudentGamificationDto();
        public StreakDto Streak { get; set; } = new StreakDto();
        public StudentStatsDto Stats { get; set; } = new StudentStatsDto();
        public DateTime JoinedAt { get; set; }
    }
}