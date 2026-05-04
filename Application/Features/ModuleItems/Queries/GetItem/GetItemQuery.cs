using Application.Features.ModuleItems.Dtos;

namespace Application.Features.ModuleItems.Queries.GetItem
{
    public sealed record GetItemQuery(int CourseId, int ModuleId, int ItemId) : IRequest<OneOf<ItemDto, Error>>;
}