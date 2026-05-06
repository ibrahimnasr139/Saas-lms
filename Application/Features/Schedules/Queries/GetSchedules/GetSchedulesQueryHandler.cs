using Application.Features.Schedules.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Schedules.Queries.GetSchedules
{
    internal sealed class GetSchedulesQueryHandler : IRequestHandler<GetSchedulesQuery, List<ScheduleDto>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IScheduleRepository _scheduleRepository;

        public GetSchedulesQueryHandler(IHttpContextAccessor httpContextAccessor, IScheduleRepository scheduleRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _scheduleRepository = scheduleRepository;
        }
        public async Task<List<ScheduleDto>> Handle(GetSchedulesQuery request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            return await _scheduleRepository.GetSchedulesAsync(subDomain!, request.Q, request.Type, request.CourseId, request.From, request.To, cancellationToken);
        }
    }
}