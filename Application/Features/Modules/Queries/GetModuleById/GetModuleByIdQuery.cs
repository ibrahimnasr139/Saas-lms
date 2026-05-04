using Application.Features.Modules.Dtos;

namespace Application.Features.Modules.Queries.GetModuleById
{
    public sealed record GetModuleByIdQuery(int CourseId, int ModuleId) : IRequest<OneOf<ModuleDto, Error>>;
}