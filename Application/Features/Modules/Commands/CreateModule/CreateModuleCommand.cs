using Domain.Enums;

namespace Application.Features.Modules.Commands.CreateModule
{
    public sealed record CreateModuleCommand(int CourseId, string Title, string Description, int Order, CourseStatus Status, bool IsFree)
        : IRequest<OneOf<SuccessDto, Error>>;
}