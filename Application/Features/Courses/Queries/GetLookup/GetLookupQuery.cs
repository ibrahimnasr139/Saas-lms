using Application.Features.Courses.Dtos;

namespace Application.Features.Courses.Queries.GetLookup
{
    public sealed record GetLookupQuery : IRequest<IEnumerable<LookupDto>>;
}
