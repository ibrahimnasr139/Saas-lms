namespace Application.Features.ModuleItems.Commands.ReorderModuleItem
{
    public sealed record ReorderModuleItemCommand(int CourseId, int ModuleId, IEnumerable<OrderDto> Orders) : IRequest<OneOf<bool, Error>>;
    public sealed class OrderDto
    {
        public int Id { get; set; }
        public int Order { get; set; }
    }
}
