using Application.Common;
using Application.Constants;
using Application.Features.Assignments.Queries.GetSubmissions;
using Application.Features.Submissions.Commands.CreateSubmissionGrade;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/tenant/courses/{courseId}/modules/{moduleId}/items/{itemId}/assignment/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthConstants.ApiScheme)]
    public class SubmissionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public SubmissionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetSubmissions([FromRoute] GetSubmissionsQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }
        
        
        [HttpPost("{submissionId}/grade")]
        public async Task<IActionResult> GradeSubmission(int courseId, int moduleId, int itemId, int submissionId, [FromBody] CreateSubmissionGradeCommand command)
        {
            command = command with { CourseId = courseId, ModuleId = moduleId, ItemId = itemId, SubmissionId = submissionId };
            var result = await _mediator.Send(command);
            return result.Match(
                success => Created(),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }
    }
}