using Application.Features.Schedules.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Schedules.Commands.DeleteSchedule
{
    internal sealed class DeleteScheduleCommandHandler : IRequestHandler<DeleteScheduleCommand, OneOf<ScheduleResponse, Error>>
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteScheduleCommandHandler(IScheduleRepository scheduleRepository, IHttpContextAccessor httpContextAccessor)
        {
            _scheduleRepository = scheduleRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<OneOf<ScheduleResponse, Error>> Handle(DeleteScheduleCommand request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var isDeleted = await _scheduleRepository.DeleteScheduleAsync(request.ScheduleId, subDomain!, cancellationToken);
            if (!isDeleted)
                return ScheduleErrors.ScheduleNotFound;
            return new ScheduleResponse { Message = MessagesConstants.ScheduleDeleted };
        }
    }
}