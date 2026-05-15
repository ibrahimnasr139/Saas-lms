using Domain.Enums;

namespace Application.Features.Public.Dtos
{
    public sealed class WebsiteCourseDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public string? Curriculum { get; set; }
        public decimal Price { get; set; }
        public string? Video { get; set; }
        public string Thumbnail { get; set; } = string.Empty;
        public byte? Discount { get; set; }
        public string Year { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public PricingType PricingType { get; set; }
        public BillingCycle? BillingCycle { get; set; }
        public List<string>? Tags { get; set; } = [];
        public string Subject { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public int StudentsCount { get; set; }
        public int LessonsCount { get; set; }
        public int ModulesCount { get; set; }
        public bool IsEnrolled { get; set; }
        public bool HasPendingOrder { get; set; }
        public CourseInstructorDto Instructor { get; set; } = new();
        public List<CourseModuleDto> Modules { get; set; } = [];
        public bool CanEnroll { get; set; }
        public string? Reason { get; set; } 
    }
}
