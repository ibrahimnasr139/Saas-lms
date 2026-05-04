namespace Application.Features.ModuleItems.Commands.UpdateModuleItem
{
    public sealed record UpdateModuleItemCommand(int CourseId, int ModuleId, int ItemId, string? Title, string? Description)
        : IRequest<OneOf<SuccessDto, Error>>;
}