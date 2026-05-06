namespace Application.Features.Lessons.Dtos
{
    public sealed class LessonPerformanceDto
    {
        public IEnumerable<ViewsOverTime>? ViewsOverTime { get; set; }
    }
}
