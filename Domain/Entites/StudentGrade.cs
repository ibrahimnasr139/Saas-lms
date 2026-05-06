using Domain.Enums;

namespace Domain.Entites
{
    public sealed class StudentGrade
    {
        public int Id { get; set; }
        public double Score { get; set; }
        public int TotalMarks { get; set; }
        public DateTime GradedAt { get; set; } = DateTime.UtcNow;
        public StudentGradeType Type { get; set; }
        public int GraderId { get; set; }
        public TenantMember TenantMember { get; set; } = null!;
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;
        public int TypeId { get; set; }
        public ModuleItem ModuleItem { get; set; } = null!;
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;
    }
}
