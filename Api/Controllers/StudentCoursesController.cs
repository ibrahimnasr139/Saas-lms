using Application.Common;
using Application.Features.StudentCourse.Queries.GetStudentCourse;
using Application.Features.StudentCourse.Queries.GetStudentCourseLiveSession;
using Application.Features.StudentCourse.Queries.GetStudentCourseLiveSessions;
using Application.Features.StudentCourse.Queries.GetStudentCourseModules;
using Application.Features.StudentCourse.Queries.GetStudentCourses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/student/courses")]
    [ApiController]
    [Authorize]
    public class StudentCoursesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public StudentCoursesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentCourses(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetStudentCoursesQuery(), cancellationToken);
            return result.Match(
                courses => Ok(courses),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("{courseId}")]
        public async Task<IActionResult> GetStudentCourse([FromRoute] int courseId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetStudentCourseQuery(courseId), cancellationToken);
            return result.Match(
                course => Ok(course),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("{courseId}/modules")]
        public async Task<IActionResult> GetStudentCourseModules([FromRoute] int courseId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetStudentCourseModulesQuery(courseId), cancellationToken);
            return result.Match(
                modules => Ok(modules),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("{courseId}/live-sessions")]
        public async Task<IActionResult> GetStudentCourseLiveSessions([FromRoute] int courseId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetStudentCourseLiveSessionsQuery(courseId), cancellationToken);
            return result.Match(
                liveSessions => Ok(liveSessions),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("{courseId}/live-sessions/{sessionId}")]
        public async Task<IActionResult> GetStudentCourseLiveSession([FromRoute] int courseId, [FromRoute] int sessionId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetStudentCourseLiveSessionQuery(courseId, sessionId), cancellationToken);
            return result.Match(
                liveSession => Ok(liveSession),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }
    }
}