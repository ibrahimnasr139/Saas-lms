namespace Application.Features.Attempts.Dtos
{
    public sealed class StudentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ProfilePicture { get; set; }
    }
}
