using Application.Common;
using Application.Constants;
using Application.Features.Students.Commands.SendReminder;
using Application.Features.TenantStudents.Commands.DeleteStudent;
using Application.Features.TenantStudents.Commands.InviteStudent;
using Application.Features.TenantStudents.Queries.GetStudent;
using Application.Features.TenantStudents.Queries.GetStudentsByCourseId;
using Application.Features.TenantStudents.Queries.GetStudentStatistics;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/tenant/students")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthConstants.ApiScheme)]
    public class TenantStudentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TenantStudentsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("send-reminder")]
        public async Task<IActionResult> SendReminder([FromBody] SendReminderCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }


        [HttpGet]
        public async Task<IActionResult> GetStudents([FromQuery] GetStudentsQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return result.Match<IActionResult>(
                student => Ok(student),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("{studentId}")]
        public async Task<IActionResult> GetStudentById([FromRoute] GetStudentQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return result.Match<IActionResult>(
                student => Ok(student),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("statistics")]
        public async Task<IActionResult> GetStudentStatistics(CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetStudentStatisticsQuery(), cancellationToken));
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteStudent([FromQuery] DeleteStudentCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                response => Ok(response),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("invite")]
        public async Task<IActionResult> InviteStudent([FromBody] InviteStudentCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                response => Ok(response),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }
    }
}
