using Application.Common;
using Application.Features.StudyTools.Commands.AskAi;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/student")]
    [ApiController]
    [Authorize]
    public class StudyToolsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StudyToolsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("ask-ai")]
        public async Task<IActionResult> AskAi([FromBody] AskAiCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                response => Ok(response),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }
    }
}