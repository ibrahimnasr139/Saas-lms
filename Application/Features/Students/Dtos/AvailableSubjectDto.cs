namespace Application.Features.Students.Dtos
{
    public sealed class AvailableSubjectDto
    {
        public int Id { get; set; }
        public string Grade { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
    }
}