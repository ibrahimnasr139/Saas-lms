using Domain.Enums;

namespace Application.Features.Modules.Dtos
{
    public sealed class AllModulesDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public CourseStatus Status { get; set; }
        public int TotalItems { get; set; }
        public int Lessons { get; set; }
        public int Assignments { get; set; }
        public int Quizzes { get; set; }
    }
}
