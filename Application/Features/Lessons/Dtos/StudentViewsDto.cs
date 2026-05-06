using Domain.Enums;

namespace Application.Features.Lessons.Dtos
{
    public sealed class StudentViewsDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = null!;
        public string? ProfilePicture { get; set; }
        public ViewStatus Status { get; set; } = ViewStatus.NotStarted;
        public int? LatestProgress { get; set; }
        public int TotalWatchTime { get; set; }
        public DateTime? LastViewTime { get; set; }
        public string? Device { get; set; }
    }
}
