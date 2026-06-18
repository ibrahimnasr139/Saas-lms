namespace Application.Features.Website.Dtos
{
    public sealed class CourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public decimal Price { get; set; }
        public string PricingType { get; set; } = string.Empty;
        public string? BillingCycle { get; set; }
        public string? Semester { get; set; }
    }
}
