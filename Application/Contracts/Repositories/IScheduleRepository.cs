using Application.Features.Schedules.Dtos;
using Domain.Enums;

namespace Application.Contracts.Repositories
{
    public interface IScheduleRepository
    {
        Task CreateScheduleAsync(Schedule schedule, CancellationToken cancellationToken);
        Task<bool> HasConflictAsync(string subDomain, DateTime start, DateTime end, bool allDay, CancellationToken cancellationToken, int? scheduleId = null);
        Task<Schedule?> GetScheduleByIdAsync(int scheduleId, string subDomain, CancellationToken cancellationToken);
        Task UpdateScheduleAsync(Schedule schedule, CancellationToken cancellationToken);
        Task<bool> DeleteScheduleAsync(int scheduleId, string subDomain, CancellationToken cancellationToken);
        Task<List<ScheduleDto>> GetSchedulesAsync(string subDomain, string? Q, ScheduleType? Type, int? CourseId, DateOnly From, DateOnly To, CancellationToken cancellationToken);
    }
}