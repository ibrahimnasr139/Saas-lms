using Application.Common;
using Application.Features.StudentAssignments.Commands.SubmitAssignment;
using Application.Features.StudentAssignments.Queries.GetStudentAssignment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/student/courses/{courseId}/items/{itemId}/assignment")]
    [ApiController]
    [Authorize]
    public class StudentAssignmentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StudentAssignmentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentAssignment([FromRoute] int courseId, [FromRoute] int itemId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetStudentAssignmentQuery(courseId, itemId), cancellationToken);
            return result.Match(
                assignment => Ok(assignment),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("submit")]
        public async Task<IActionResult> SubmitAssignment([FromRoute] int courseId, [FromRoute] int itemId, [FromBody] SubmitAssignmentCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command with { CourseId = courseId, ItemId = itemId }, cancellationToken);
            return result.Match(
                assignment => Ok(assignment),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }
    }
}