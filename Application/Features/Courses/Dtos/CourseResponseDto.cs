using Domain.Enums;

namespace Application.Features.Courses.Dtos
{
    public sealed class CourseResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Grade { get; set; } = null!;
        public decimal Price { get; set; }
        public string ThumbnailUrl { get; set; } = null!;
        public CourseStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? LessonsCount { get; set; }
        public int? StudentsCount { get; set; }
        public double? CompletionRate { get; set; }
    }
}
