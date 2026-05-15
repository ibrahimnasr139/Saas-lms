using Application.Common;
using Application.Features.Students.Commands.AcceptInvite;
using Application.Features.Students.Commands.DeclineInvite;
using Application.Features.Students.Commands.ValidateStudentInvite;
using Application.Features.Students.Queries.GetAvailableSubjects;
using Application.Features.Students.Queries.GetSubjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Api.Controllers
{
    [Route("api/student")]
    [ApiController]
    [Authorize]
    public class StudentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StudentsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("invites/validate")]
        public async Task<IActionResult> ValidateInvite([FromQuery] ValidateStudentInviteCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                response => Ok(response),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("invites/accept")]
        public async Task<IActionResult> AcceptInvite([FromQuery] AcceptInviteCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                response => Ok(response),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("invites/decline")]
        public async Task<IActionResult> DeclineInvite([FromQuery] DeclineInviteCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                response => Ok(response),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("available-subjects")]
        public async Task<IActionResult> GetAvailableSubjects(CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetAvailableSubjectsQuery(), cancellationToken));
        }


        [HttpGet("subjects")]
        public async Task<IActionResult> GetSubjects(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetSubjectsQuery(), cancellationToken);
            return result.Match<IActionResult>(
                response => Ok(response),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }
    }
}