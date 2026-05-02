using Application.Common;
using Application.Constants;
using Application.Features.Courses.Commands.CreateCourse;
using Application.Features.Courses.Commands.DeleteCourse;
using Application.Features.Courses.Commands.UpdateCourse;
using Application.Features.Courses.Queries.GetAll;
using Application.Features.Courses.Queries.GetCourse;
using Application.Features.Courses.Queries.GetCourseStatistics;
using Application.Features.Courses.Queries.GetLookup;
using Application.Features.Courses.Queries.GetStatistics;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/tenant/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthConstants.ApiScheme)]
    public class CoursesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CoursesController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics(CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetStatisticsQuery(), cancellationToken));
        }


        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllQuery getAllQuery, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(getAllQuery, cancellationToken));
        }


        [HttpGet("{courseId}")]
        public async Task<IActionResult> GetById([FromRoute] int courseId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCourseQuery(courseId), cancellationToken);
            return result.Match(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("lookup")]
        public async Task<IActionResult> GetAllForLookup(CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetLookupQuery(), cancellationToken));
        }


        [HttpGet("{courseId}/statistics")]
        public async Task<IActionResult> GetCourseStatisticsByCourseId([FromRoute] GetCourseStatisticsQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return result.Match(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost]
        public async Task<IActionResult> CreateCourse(CreateCourseCommand createCourseCommand, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(createCourseCommand, cancellationToken);
            return result.Match(
                success => Created(string.Empty, success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPut("{courseId}")]
        public async Task<IActionResult> UpdateCourse([FromRoute] int courseId, UpdateCourseCommand updateCourseCommand, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(updateCourseCommand with { CourseId = courseId }, cancellationToken);
            return result.Match(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpDelete("{courseId}")]
        public async Task<IActionResult> DeleteCourse([FromRoute] int courseId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteCourseCommand(courseId), cancellationToken);
            return result.Match(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }
    }
}
