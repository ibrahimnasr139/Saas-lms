using Application.Common;
using Application.Constants;
using Application.Features.Lessons.Commands.UpdateLesson;
using Application.Features.Lessons.Queries.GetLessonOverview;
using Application.Features.Lessons.Queries.GetLessonPerformance;
using Application.Features.Lessons.Queries.GetViews;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/tenant/courses/{courseId}/modules/{moduleId}/items/{itemId}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthConstants.ApiScheme)]
    public class LessonController : ControllerBase
    {
        private readonly IMediator _mediator;
        public LessonController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("views")]
        public async Task<IActionResult> GetViews([FromRoute] GetViewsQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }
        
        
        [HttpGet("performance")]
        public async Task<IActionResult> GetPerformance([FromRoute] GetLessonPerformanceQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }
        
        
        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview([FromRoute] GetLessonOverviewQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }
        
        
        [HttpPatch]
        public async Task<IActionResult> UpdateLesson(int courseId, int moduleId, int itemId, [FromBody] UpdateLessonCommand command, CancellationToken cancellationToken)
        {
            command = command with { CourseId = courseId, ModuleId = moduleId, ItemId = itemId };
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }
    }
}
