using Domain.Enums;

namespace Application.Features.Tenants.Dtos
{
    public sealed class LiveSessionDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public CourseDto Course { get; set; } = new CourseDto();
        public DateTime DateTime { get; set; }
        public int Duration { get; set; }
        public string Teacher { get; set; } = string.Empty;
        public LiveSessionStatus Status { get; set; }
        public int? Attendance { get; set; }
        public bool Recorded { get; set; }
        public string JoinUrl { get; set; } = string.Empty;
        public int TotalStudents { get; set; }
        public NotificationDto Notifications { get; set; } = new NotificationDto();
        public LiveSessionSettingsDto Settings { get; set; } = new LiveSessionSettingsDto();
    }
}
