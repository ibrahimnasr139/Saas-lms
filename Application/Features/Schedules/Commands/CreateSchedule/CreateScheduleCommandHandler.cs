using Application.Contracts.Repositories;
using Application.Features.Schedules.Dtos;
using Microsoft.AspNetCore.Http;
using TimeZoneConverter;

namespace Application.Features.Schedules.Commands.CreateSchedule
{
    internal sealed class CreateScheduleCommandHandler : IRequestHandler<CreateScheduleCommand, OneOf<ScheduleResponse, Error>>
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantRepository _tenantRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CreateScheduleCommandHandler(IScheduleRepository scheduleRepository, IHttpContextAccessor httpContextAccessor,
            ITenantRepository tenantRepository, ICourseRepository courseRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _scheduleRepository = scheduleRepository;
            _httpContextAccessor = httpContextAccessor;
            _tenantRepository = tenantRepository;
            _courseRepository = courseRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<ScheduleResponse, Error>> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);
            var course = await _courseRepository.GetCourseByIdAsync(request.CourseId, subDomain!, cancellationToken);
            if (course is null)
                return CourseErrors.CourseNotFound;

            var startAt = request.Start;
            var endAt = request.End;

            if (request.AllDay)
            {
                var egyptTimeZone = TZConvert.GetTimeZoneInfo("Egypt Standard Time");
                var startEgypt = TimeZoneInfo.ConvertTimeFromUtc(startAt, egyptTimeZone);
                var date = startEgypt.Date;
                var startOfDayEgypt = date;
                var endOfDayEgypt = date.AddDays(1);

                startAt = TimeZoneInfo.ConvertTimeToUtc(startOfDayEgypt, egyptTimeZone);
                endAt = TimeZoneInfo.ConvertTimeToUtc(endOfDayEgypt, egyptTimeZone);
            }

            var hasConflict = await _scheduleRepository.HasConflictAsync(subDomain!, startAt, endAt, request.AllDay, cancellationToken);
            if (hasConflict)
                return ScheduleErrors.ScheduleConflict;

            var schedule = _mapper.Map<Schedule>(request);
            schedule.StartAt = startAt;
            schedule.EndAt = endAt;
            schedule.TenantId = tenantId;
            schedule.CourseId = request.CourseId;

            await _scheduleRepository.CreateScheduleAsync(schedule, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            return new ScheduleResponse { Message = MessagesConstants.ScheduleCreated };
        }
    }
}