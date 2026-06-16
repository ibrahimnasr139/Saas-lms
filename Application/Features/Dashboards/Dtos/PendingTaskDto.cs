using System.Text.Json.Serialization;

namespace Application.Features.Dashboards.Dtos
{
    public sealed class PendingTaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Count { get; set; }
        public int CourseId { get; set; }
        public int ModuleId { get; set; }
        [JsonIgnore]
        public DateTime? LastSubmittedAt { get; set; }
    }
}