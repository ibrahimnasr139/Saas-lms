using Application.Common;
using Application.Constants;
using Application.Features.Assignments.Commands.UpdateAssignment;
using Application.Features.Assignments.Queries.GetAssignment;
using Application.Features.Assignments.Queries.GetOverview;
using Application.Features.Assignments.Queries.GetPerformance;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/tenant/courses/{courseId}/modules/{moduleId}/items/{itemId}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthConstants.ApiScheme)]
    public class AssignmentController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AssignmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateAssignment(int courseId, int moduleId, int itemId, [FromBody] UpdateAssignmentCommand command, CancellationToken cancellationToken)
        {
            command = command with { CourseId = courseId, ModuleId = moduleId, ItemId = itemId };
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }


        [HttpGet("content")]
        public async Task<IActionResult> GetAssignmentContent([FromRoute] GetAssignmentQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }


        [HttpGet("overview")]
        public async Task<IActionResult> GetAssignmentOverview([FromRoute] GetOverviewQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }


        [HttpGet("performance")]
        public async Task<IActionResult> GetAssignmentPerformance([FromRoute] GetPerformanceQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }
    }
}
