using Application.Common;
using Application.Constants;
using Application.Features.Dashboards.Queries.GetPendingTasks;
using Application.Features.Dashboards.Queries.GetQuickAnalytics;
using Application.Features.Dashboards.Queries.GetTopStudentsPerformance;
using Application.Features.Dashboards.Queries.GetUpcomingSessions;
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


        [HttpGet("quick-analytics")]
        public async Task<IActionResult> GetQuickAnalytics(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetQuickAnalyticsQuery(), cancellationToken);
            return result.Match<IActionResult>(
               success => Ok(success),
               error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("upcoming-sessions")]
        public async Task<IActionResult> GetUpcomingSessions(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetUpcomingSessionsQuery(), cancellationToken);
            return result.Match<IActionResult>(
               success => Ok(success),
               error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("top-students-performance")]
        public async Task<IActionResult> GetTopStudentsPerformance(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetTopStudentsPerformanceQuery(), cancellationToken);
            return result.Match<IActionResult>(
               success => Ok(success),
               error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }
    }
}