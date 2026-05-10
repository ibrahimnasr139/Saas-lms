using Application.Features.Tenants.Dtos;

namespace Application.Features.Tenants.Commands.CreateOnboarding
{
    public sealed record CreateOnboardingCommand(string PlatformName, string SubDomain, string DisplayName, string JobTitle,
        int ExperienceYears, IEnumerable<LabelValueDto> Subjects, IEnumerable<LabelValueDto> TeachingLevels,
        IEnumerable<LabelValueDto> Grades, string? Bio, string? ProfilePicture, string? Logo) : IRequest<OneOf<OnboardingDto, Error>>;
}