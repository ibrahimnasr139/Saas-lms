using Application.Features.TenantMembers.Dtos;
using Application.Features.Tenants.Dtos;

namespace Application.Features.TenantMembers.Commands.UpdateCurrentMember
{
    public sealed record UpdateCurrentMemberCommand(string? FirstName, string? LastName, string? Email, string? Phone, string? DisplayName,
        int? ExperienceYears, string? JobTitle, string? ProfilePicture, string? Bio, List<LabelValueDto>? Subjects, List<LabelValueDto>? TeachingLevels,
        List<LabelValueDto>? Grades) : IRequest<UpdateCurrentMemberDto>;
}