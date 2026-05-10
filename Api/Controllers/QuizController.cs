using Application.Common;
using Application.Constants;
using Application.Features.Quizzes.Commands.CreateAiQuizQuestions;
using Application.Features.Quizzes.Commands.UpdateQuiz;
using Application.Features.Quizzes.Queries.GetAttempts;
using Application.Features.Quizzes.Queries.GetOverview;
using Application.Features.Quizzes.Queries.GetPerformance;
using Application.Features.Quizzes.Queries.GetQuiz;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/tenant/courses/{courseId}/modules/{moduleId}/items/{itemId}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthConstants.ApiScheme)]
    public class QuizController : ControllerBase
    {
        private readonly IMediator _mediator;
        public QuizController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPatch]
        public async Task<IActionResult> UpdateQuiz(int courseId, int moduleId, int itemId, [FromBody] UpdateQuizCommand command, CancellationToken cancellationToken)
        {
            command = command with { CourseId = courseId, ModuleId = moduleId, ItemId = itemId };
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }


        [HttpGet("content")]
        public async Task<IActionResult> GetQuizContent([FromRoute] GetQuizQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("attempts")]
        public async Task<IActionResult> GetAttempts([FromRoute] GetAttemptsQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview([FromRoute] GetOverviewQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }


        [HttpGet("performance")]
        public async Task<IActionResult> GetPerformance([FromRoute] GetPerformanceQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }


        [HttpPost("ai-generate-questions")]
        public async Task<IActionResult> CreateAiQuizQuestions(int courseId, int moduleId, int itemId, [FromBody] CreateAiQuizQuestionsCommand command, CancellationToken cancellationToken)
        {
            command = command with { CourseId = courseId, ModuleId = moduleId, ItemId = itemId };
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                success => Created(),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }
    }
}