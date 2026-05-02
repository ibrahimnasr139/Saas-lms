using Application.Common;
using Application.Constants;
using Application.Features.Tenants.Commands.CreateLiveSession;
using Application.Features.Tenants.Commands.DeleteContentLibraryResource;
using Application.Features.Tenants.Commands.DeleteLiveSession;
using Application.Features.Tenants.Commands.UpdateLiveSession;
using Application.Features.Tenants.Queries.GetContentLibraryResources;
using Application.Features.Tenants.Queries.GetContentLibraryStatistics;
using Application.Features.Tenants.Queries.GetLastTenant;
using Application.Features.Tenants.Queries.GetLiveSession;
using Application.Features.Tenants.Queries.GetLiveSessions;
using Application.Features.Tenants.Queries.GetLiveSessionsStatistics;
using Application.Features.Tenants.Queries.GetTenantPermissions;
using Application.Features.Tenants.Queries.GetTenantUsage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/tenant")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthConstants.ApiScheme)]

    public class TenantsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TenantsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetLastTenant(CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetLastTenantQuery(), cancellationToken));
        }


        [HttpGet("usage")]
        public async Task<IActionResult> GetTenantUsage(CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetTenantUsageQuery(), cancellationToken));
        }


        [HttpGet("content-library/resources")]
        public async Task<IActionResult> GetContentLibraryResources([FromQuery] string? q, [FromQuery] string type, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetContentLibraryResourcesQuery(q, type), cancellationToken));
        }


        [HttpGet("content-library/statistics")]
        public async Task<IActionResult> GetContentLibraryStatistics(CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetContentLibraryStatisticsQuery(), cancellationToken));
        }


        [HttpDelete("content-library/resources/{id}")]
        public async Task<IActionResult> DeleteContentLibraryResource([FromRoute] string id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteContentLibraryResourceCommand(id), cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("permissions")]
        public async Task<IActionResult> GetTenantPermissions(CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new TenantPermissionsQuery(), cancellationToken));
        }


        [HttpGet("live-sessions")]
        public async Task<IActionResult> GetLiveSessions(CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetLiveSessionsQuery(), cancellationToken));
        }


        [HttpPost("live-sessions")]
        public async Task<IActionResult> CreateLiveSession([FromBody] CreateLiveSessionCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new CreateLiveSessionCommand(command.Title, command.Description, command.CourseId,
                command.ScheduledAt, command.Duration, command.Settings, command.Notifications), cancellationToken);

            return result.Match(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("live-sessions/{sessionsId}")]
        public async Task<IActionResult> GetLiveSession([FromRoute] int sessionsId, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetLiveSessionQuery(sessionsId), cancellationToken));
        }


        [HttpPut("live-sessions/{sessionId}")]
        public async Task<IActionResult> UpdateSession([FromRoute] int sessionId, [FromBody] UpdateLiveSessionCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new UpdateLiveSessionCommand(sessionId, command.Title, command.Description, command.CourseId,
                command.ScheduledAt, command.Duration, command.Settings, command.Notifications), cancellationToken);

            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpDelete("live-sessions/{sessionsId}")]
        public async Task<IActionResult> DeleteLiveSession([FromRoute] int sessionsId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteLiveSessionCommand(sessionsId), cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("live-sessions/statistics")]
        public async Task<IActionResult> GetLiveSessionsStatistics(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetLiveSessionsStatisticsQuery(), cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }
    }
}
