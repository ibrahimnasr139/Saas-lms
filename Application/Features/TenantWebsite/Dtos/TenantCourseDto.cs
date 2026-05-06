namespace Application.Features.TenantWebsite.Dtos
{
    public sealed class TenantCourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public byte? Discount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string PricingType { get; set; } = string.Empty;
        public string? BillingCycle { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string? Curriculum { get; set; }
        public string? Semester { get; set; }
        public string Year { get; set; } = string.Empty;
        public List<string>? Tags { get; set; } = [];
        public bool IsPublished { get; set; }
        public int StudentsCount { get; set; }
        public int LessonsCount { get; set; }
        public int Rating { get; set; }
        public InstructorDto? Instructor { get; set; }
    }
}