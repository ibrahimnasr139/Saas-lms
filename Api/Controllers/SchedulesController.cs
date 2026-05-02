using Application.Common;
using Application.Constants;
using Application.Features.Schedules.Commands.CreateSchedule;
using Application.Features.Schedules.Commands.DeleteSchedule;
using Application.Features.Schedules.Commands.UpdateSchedule;
using Application.Features.Schedules.Queries.GetSchedules;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/tenant/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthConstants.ApiScheme)]
    public class SchedulesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SchedulesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetSchedules([FromQuery] GetSchedulesQuery query, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(query, cancellationToken));
        }


        [HttpPost]
        public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                scheduleResponse => Ok(scheduleResponse),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPut("{scheduleId}")]
        public async Task<IActionResult> UpdateSchedule([FromRoute] int scheduleId, [FromBody] UpdateScheduleCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command with { ScheduleId = scheduleId }, cancellationToken);
            return result.Match<IActionResult>(
                scheduleResponse => Ok(scheduleResponse),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpDelete("{scheduleId}")]
        public async Task<IActionResult> DeleteSchedule([FromRoute] DeleteScheduleCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                scheduleResponse => Ok(scheduleResponse),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }
    }
}