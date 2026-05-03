using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entites
{
    public sealed class Order : IAuditable
    {
        public int Id { get; set; }
        public string? OrderNumber { get; set; }
        public decimal PricePaid { get; set; }
        public string PaymentProof { get; set; } = string.Empty;
        public string? PaymentReference { get; set; }
        public string? RejectionReason { get; set; }
        public PaymentMethodType PaymentType { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? DeclinedAt { get; set; }
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;
        public ICollection<OrderTimeLine> OrderTimeLines { get; set; } = [];
        public ICollection<Enrollment> Enrollments { get; set; } = [];
    }
}
