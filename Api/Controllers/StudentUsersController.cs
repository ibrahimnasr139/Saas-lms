using Application.Common;
using Application.Features.StudentUsers.Commands.Onboarding;
using Application.Features.StudentUsers.Queries.GetCurrentStudent;
using Application.Features.StudentUsers.Queries.GetProfile;
using Application.Features.StudentUsers.Queries.GetStudentStreak;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/student")]
    [ApiController]
    [Authorize]
    public class StudentUsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StudentUsersController(IMediator mediator)
        {
            _mediator = mediator;
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
        public async Task<IActionResult> GetCurrentStudent(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCurrentStudentQuery(), cancellationToken);
            return result.Match<IActionResult>(
                student => Ok(student),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }
    }
}