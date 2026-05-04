using Domain.Enums;

namespace Application.Features.Modules.Commands.UpdateModule
{
    public sealed record UpdateModuleCommand(int CourseId, int ModuleId, string Title, string Description, int Order, CourseStatus Status, bool IsFree)
        : IRequest<OneOf<SuccessDto, Error>>;
}