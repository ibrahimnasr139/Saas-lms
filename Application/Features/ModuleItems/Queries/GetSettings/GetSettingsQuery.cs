using Application.Features.ModuleItems.Dtos;

namespace Application.Features.ModuleItems.Queries.GetSettings
{
    public sealed record GetSettingsQuery(int CourseId, int ModuleId, int ItemId) : IRequest<OneOf<SettingsDto, Error>>;

}
