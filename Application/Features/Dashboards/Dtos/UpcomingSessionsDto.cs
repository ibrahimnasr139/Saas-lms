using Application.Features.Tenants.Dtos;
using Domain.Enums;
namespace Application.Features.Dashboards.Dtos
{
    public sealed class UpcomingSessionsDto
    {
        public int Id { get; set; }
        public CourseDto Course { get; set; } = new CourseDto();
        public string SessionTitle { get; set; } = string.Empty;
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int StudentsEnrolled { get; set; }
    }
}