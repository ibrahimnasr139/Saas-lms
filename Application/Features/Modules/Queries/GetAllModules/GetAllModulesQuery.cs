using Application.Features.Modules.Dtos;

namespace Application.Features.Modules.Queries.GetAllModules
{
    public sealed record GetAllModulesQuery(int CourseId) : IRequest<OneOf<IEnumerable<AllModulesDto>, Error>>;
}