using Application.Features.Dashboards.Dtos;

namespace Application.Features.Dashboards.Queries.GetUpcomingSessions
{
    public sealed record GetUpcomingSessionsQuery : IRequest<OneOf<List<UpcomingSessionsDto>, Error>>;
}