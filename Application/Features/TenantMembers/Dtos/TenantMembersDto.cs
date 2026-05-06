namespace Application.Features.TenantMembers.Dtos
{
    public sealed class TenantMembersDto
    {
        public int Id { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ProfilePicture { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
    }
}
