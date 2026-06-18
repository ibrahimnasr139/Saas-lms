using Application.Common;
using Application.Constants;
using Application.Features.Questions.Commands.CreateQuizQuestion;
using Application.Features.Questions.Commands.DeleteQuizQuestion;
using Application.Features.Questions.Commands.DuplicateQuizQuestion;
using Application.Features.Questions.Commands.ImportQuizQuestion;
using Application.Features.Questions.Commands.ReorderQuestion;
using Application.Features.Questions.Commands.UpdateQuizQuestion;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/tenant/courses/{courseId}/modules/{moduleId}/items/{itemId}/quiz/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthConstants.ApiScheme)]
    public class QuestionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public QuestionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportQuestions(int courseId, int moduleId, int itemId, [FromBody] ImportQuizQuestionCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command with { CourseId = courseId, ModuleId = moduleId, ItemId = itemId }, cancellationToken);
            return result.Match<IActionResult>(
                success => Created(),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost]
        public async Task<IActionResult> CreateQuestion(int courseId, int moduleId, int itemId, [FromBody] CreateQuizQuestionCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command with { CourseId = courseId, ModuleId = moduleId, ItemId = itemId }, cancellationToken);
            return result.Match<IActionResult>(
                success => Created(),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpDelete("{questionId}")]
        public async Task<IActionResult> DeleteQuestion([FromRoute] DeleteQuizQuestionCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("{questionId}/duplicate")]
        public async Task<IActionResult> DuplicateQuestion([FromRoute] DuplicateQuizQuestionCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }


        [HttpPut("{questionId}")]
        public async Task<IActionResult> UpdateQuestion(int courseId, int moduleId, int itemId, int questionId, [FromBody] UpdateQuizQuestionCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command with { CourseId = courseId, ModuleId = moduleId, ItemId = itemId, QuestionId = questionId }, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }


        [HttpPost("reorder")]
        public async Task<IActionResult> ReorderQuestions(int courseId, int moduleId, int itemId, [FromBody] ReorderQuestionCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command with { CourseId = courseId, ModuleId = moduleId, ItemId = itemId }, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }
    }
}