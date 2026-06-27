namespace Application.Features.Students.Dtos
{
    public sealed class ProfileDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Avatar { get; set; }
    }
}