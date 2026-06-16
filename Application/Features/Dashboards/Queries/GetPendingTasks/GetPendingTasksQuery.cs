using Application.Features.Dashboards.Dtos;

namespace Application.Features.Dashboards.Queries.GetPendingTasks
{
    public sealed record GetPendingTasksQuery : IRequest<OneOf<List<PendingTaskDto>, Error>>;
}