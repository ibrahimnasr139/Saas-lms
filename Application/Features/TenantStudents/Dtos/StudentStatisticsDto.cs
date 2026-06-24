namespace Application.Features.TenantStudents.Dtos
{
    public sealed class StudentStatisticsDto
    {
        public int Students { get; set; }
        public int AverageGrade { get; set; }
        public int AttendanceRate { get; set; }
        public int ActiveStudents { get; set; }
    }
}