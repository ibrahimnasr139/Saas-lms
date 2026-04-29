using Application.Features.Schedules.Dtos;
using AutoMapper;
using Domain.Enums;
using TimeZoneConverter;

namespace Infrastructure.Repositories
{
    internal sealed class ScheduleRepository : IScheduleRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ScheduleRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task CreateScheduleAsync(Schedule schedule, CancellationToken cancellationToken)
        {
            await _context.Schedules.AddAsync(schedule);
        }
        public async Task<bool> HasConflictAsync(string subDomain, DateTime start, DateTime end, bool allDay, CancellationToken cancellationToken, int? scheduleId = null)
        {
            var startOfDay = start.Date;
            var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

            return await _context.Schedules
                .Where(s => s.Tenant.SubDomain == subDomain && (scheduleId == null || s.Id != scheduleId))
                .AnyAsync(s => (s.AllDay && s.StartAt.Date == start.Date) ||
                    (allDay && s.StartAt < endOfDay && s.EndAt > startOfDay) ||
                    (!s.AllDay && !allDay && s.StartAt < end && s.EndAt > start),
                cancellationToken);
        }
        public async Task<Schedule?> GetScheduleByIdAsync(int scheduleId, string subDoamin, CancellationToken cancellationToken)
        {
            return await _context.Schedules
                .FirstOrDefaultAsync(s => s.Id == scheduleId && s.Tenant.SubDomain == subDoamin, cancellationToken);
        }
        public Task UpdateScheduleAsync(Schedule schedule, CancellationToken cancellationToken)
        {
            _context.Schedules.Update(schedule);
            return Task.CompletedTask;
        }
        public async Task<bool> DeleteScheduleAsync(int scheduleId, string subDomain, CancellationToken cancellationToken)
        {
            var schedule = await _context.Schedules
                .FirstOrDefaultAsync(s => s.Id == scheduleId && s.Tenant.SubDomain == subDomain, cancellationToken);
            if (schedule == null)
                return false;
            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        public async Task<List<ScheduleDto>> GetSchedulesAsync(string subDomain, string? Q, ScheduleType? Type, int? CourseId, DateOnly From, DateOnly To, CancellationToken cancellationToken)
        {
            var cairo = TZConvert.GetTimeZoneInfo("Egypt Standard Time");
            var fromLocal = From.ToDateTime(TimeOnly.MinValue);
            var toLocal = To.ToDateTime(TimeOnly.MaxValue);
            var fromUtc = TimeZoneInfo.ConvertTimeToUtc(fromLocal, cairo);
            var toUtc = TimeZoneInfo.ConvertTimeToUtc(toLocal, cairo);

            var query = _context.Schedules
                .Where(s => s.Tenant.SubDomain == subDomain)
                .Where(s => (!s.RepeatEvent && s.StartAt >= fromUtc && s.StartAt <= toUtc) ||
                    (s.RepeatEvent && s.StartAt <= toUtc && (s.RepeatPeriodEnd == null || s.RepeatPeriodEnd >= fromUtc)));

            if (CourseId != null)
                query = query.Where(s => s.CourseId == CourseId);

            if (Type != null)
                query = query.Where(s => s.Type == Type);

            if (!string.IsNullOrEmpty(Q))
                query = query.Where(s => s.Title.Contains(Q) || (s.Description != null && s.Description.Contains(Q)));

            var schedules = await query.ToListAsync(cancellationToken);

            var result = new List<ScheduleDto>();

            foreach (var schedule in schedules)
            {
                if (!schedule.RepeatEvent)
                {
                    result.Add(_mapper.Map<ScheduleDto>(schedule));
                    continue;
                }

                var duration = schedule.EndAt - schedule.StartAt;
                var current = schedule.StartAt;
                var repeatEnd = schedule.RepeatPeriodEnd ?? toUtc;

                while (current <= toUtc && current <= repeatEnd)
                {
                    if (current >= fromUtc)
                    {
                        var dto = _mapper.Map<ScheduleDto>(schedule);
                        dto.Start = current;
                        dto.End = current + duration;
                        result.Add(dto);
                    }

                    current = schedule.RepeatFrequency switch
                    {
                        ScheduleRepeatFrequency.Daily => current.AddDays(1),
                        ScheduleRepeatFrequency.Weekly => current.AddDays(7),
                        ScheduleRepeatFrequency.Monthly => current.AddMonths(1),
                        _ => repeatEnd.AddDays(1)
                    };
                }
            }
            return result;
        }
    }
}