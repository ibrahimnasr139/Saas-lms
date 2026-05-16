namespace Application.Features.Tenants.Dtos
{
    public sealed class GenerateDescriptionRequest
    {
        public ContextDto Context { get; set; } = default!;
        public string Type { get; set; } = string.Empty;
    }
    public class ContextDto
    {
        public ContextCourseDto? Course { get; set; }
        public ContextTitle? Module { get; set; }
        public ContextTitle? Lesson { get; set; }
        public ContextTitle? Quiz { get; set; }
        public ContextTitle? Assignment { get; set; }
    }
    public class ContextCourseDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Level { get; set; }
    }
    public class ContextTitle
    {
        public string Title { get; set; } = string.Empty;
    }
    public enum DescriptionType
    {
        Course,
        Module,
        Lesson,
        Quiz,
        Assignment
    }
}