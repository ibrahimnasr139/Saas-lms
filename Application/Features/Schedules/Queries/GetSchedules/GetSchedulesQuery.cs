using Application.Features.Schedules.Dtos;
using Domain.Enums;

namespace Application.Features.Schedules.Queries.GetSchedules
{
    public sealed record GetSchedulesQuery(string? Q, ScheduleType? Type, int? CourseId, DateOnly From, DateOnly To)
        : IRequest<List<ScheduleDto>>;
}