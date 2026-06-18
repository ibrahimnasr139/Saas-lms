using Domain.Enums;

namespace Application.Features.Website.Dtos
{
    public sealed class TenantOrderDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public StudentDto Student { get; set; } = new();
        public CourseDto Course { get; set; } = new();
        public PaymentMethodType PaymentMethod { get; set; }
        public string PaymentProof { get; set; } = string.Empty;
        public string? PaymentReference { get; set; }
        public decimal PricePaid { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? DeclinedAt { get; set; }
        public List<TimelineDto> Timeline { get; set; } = new();
    }
}
