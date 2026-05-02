using Application.Features.ModuleItems.Dtos;
using Domain.Enums;

namespace Application.Features.ModuleItems.Commands.UpdateSettings
{
    public sealed record UpdateSettingsCommand(int CourseId, int ModuleId, int ItemId, CourseStatus Status, bool AllowDiscussions,
         IEnumerable<ConditionDto> Conditions) : IRequest<OneOf<SuccessDto, Error>>;
}