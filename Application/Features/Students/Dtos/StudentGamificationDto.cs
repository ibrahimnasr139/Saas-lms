namespace Application.Features.Students.Dtos
{
    public sealed class StudentGamificationDto
    {
        public int Level { get; set; }
        public int Xp { get; set; }
        public int NextLevelXp { get; set; }
        public string? Title { get; set; }
        public double TotalBadges { get; set; }
        public double CompletedMissions { get; set; }
    }
}