namespace Application.Features.Students.Dtos
{
    public sealed class StudentSubjectProfileDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Confidence { get; set; }
    }
}