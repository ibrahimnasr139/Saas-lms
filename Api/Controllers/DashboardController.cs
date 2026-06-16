using Application.Common;
using Application.Constants;
using Application.Features.Dashboards.Queries.GetPendingTasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/tenant/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthConstants.ApiScheme)]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("pending-tasks")]
        public async Task<IActionResult> GetPendingTasks(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetPendingTasksQuery(), cancellationToken);
            return result.Match<IActionResult>(
               success => Ok(success),
               error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }
    }
}