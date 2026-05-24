using Application.Features.Discussions.Dtos;
using Domain.Enums;

namespace Application.Features.Discussions.Queries.GetAllDiscussions
{
    public sealed record GetAllDiscussionsQuery(string? Q, int? CourseId, ModuleItemType? Type, int? Cursor, int? Limit,
        int? ModuleId, int? ItemId) : IRequest<AllDiscussionsDto>;
}