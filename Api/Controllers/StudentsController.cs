using Application.Common;
using Application.Features.Students.Commands.AcceptInvite;
using Application.Features.Students.Commands.DeclineInvite;
using Application.Features.Students.Commands.Onboarding;
using Application.Features.Students.Commands.UpdateProfile;
using Application.Features.Students.Commands.ValidateStudentInvite;
using Application.Features.Students.Queries.GetAvailableSubjects;
using Application.Features.Students.Queries.GetCurrentStudent;
using Application.Features.Students.Queries.GetProfile;
using Application.Features.Students.Queries.GetProfileDetails;
using Application.Features.Students.Queries.GetStudentStreak;
using Application.Features.Students.Queries.GetSubjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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


        [HttpGet("users/me")]
        public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetProfileQuery(), cancellationToken);
            return result.Match<IActionResult>(
                profile => Ok(profile),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("onboarding")]
        public async Task<IActionResult> Onboarding([FromBody] OnboardingCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                response => Ok(response),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("streak")]
        public async Task<IActionResult> GetStudentStreak(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetStudentStreakQuery(), cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("current")]
        public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCurrentStudentQuery(), cancellationToken);
            return result.Match<IActionResult>(
                student => Ok(student),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPatch("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                response => Ok(new { Success = response }),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("profile")]
        public async Task<IActionResult> GetProfileDetails(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetProfileDetailsQuery(), cancellationToken);
            return result.Match<IActionResult>(
                profile => Ok(profile),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }
    }
}