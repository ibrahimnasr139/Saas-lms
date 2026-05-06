using Application.Common;
using Application.Features.StudentQuizes.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/student/courses/{courseId}/items/{itemId}/quiz")]
    [ApiController]
    public class StudentQuizesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StudentQuizesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetQuiz([FromRoute] int courseId, [FromRoute] int itemId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetStudentQuizQuery(courseId, itemId), cancellationToken);
            return result.Match(
                quiz => Ok(quiz),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }
    }
}