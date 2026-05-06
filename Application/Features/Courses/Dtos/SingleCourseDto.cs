using Domain.Enums;

namespace Application.Features.Courses.Dtos
{
    public sealed class SingleCourseDto
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int SubjectId { get; set; }
        public int GradeId { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; } = null!;
        public string Thumbnail { get; set; } = null!;
        public string? Curriculum { get; set; }
        public CourseStatus Status { get; set; }
        public PricingType PricingType { get; set; }
        public byte Discount { get; set; }
        public string[]? Tags { get; set; }
        public BillingCycle? BillingCycle { get; set; }
        public string Year { get; set; } = string.Empty;
        public string? Semester { get; set; }
        public string? Video { get; set; }

    }
}
