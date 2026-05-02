using Application.Common;
using Application.Constants;
using Application.Features.Attempts.Commands.CreateAttemptManualGrading;
using Application.Features.Attempts.Commands.PublishAttempt;
using Application.Features.Attempts.Queries.GetAttempt;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/tenant/quizzes/{quizId}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthConstants.ApiScheme)]

    public class AttemptsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AttemptsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("{attemptId}/grades")]
        public async Task<IActionResult> CreateManualGrade(int quizId, int attemptId, [FromBody] CreateAttemptManualGradeCommand command, CancellationToken cancellationToken)
        {
            command = command with { QuizId = quizId, AttemptId = attemptId };
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                success => Created(),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }
        
        [HttpPost("{attemptId}/publish")]
        public async Task<IActionResult> PublishAttempt([FromRoute] PublishAttemptCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }
        
        [HttpGet("{attemptId}")]
        public async Task<IActionResult> GetAttempt([FromRoute] GetAttemptQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return result.Match<IActionResult>(
                attempt => Ok(attempt),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }
    }
}
