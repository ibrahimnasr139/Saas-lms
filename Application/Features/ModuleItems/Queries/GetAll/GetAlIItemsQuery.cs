using Application.Features.ModuleItems.Dtos;
using Domain.Enums;

namespace Application.Features.ModuleItems.Queries.GetAll
{
    public sealed record GetAllItemsQuery(int CourseId, int ModuleId, ModuleItemType? Type, int ItemId)
        : IRequest<OneOf<IEnumerable<AllItemsDto>, Error>>;
}