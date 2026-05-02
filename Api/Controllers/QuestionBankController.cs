using Application.Common;
using Application.Constants;
using Application.Features.Questions.Commands.CreateQuestion;
using Application.Features.Questions.Commands.CreateQuestionCategory;
using Application.Features.Questions.Commands.DeleteQuestion;
using Application.Features.Questions.Commands.UpdateQuestion;
using Application.Features.Questions.Queries.GetAllQuestions;
using Application.Features.Questions.Queries.GetCategories;
using Application.Features.Questions.Queries.GetQuestion;
using Application.Features.Questions.Queries.GetStatistics;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/tenant/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthConstants.ApiScheme)]
    public class QuestionBankController : ControllerBase
    {
        private readonly IMediator _mediator;
        public QuestionBankController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateQuestionCategoryCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                success => Created(),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }
       
        
        [HttpDelete("questions/{questionId}")]
        public async Task<IActionResult> DeleteQuestion([FromRoute] DeleteQuestionCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                success => NoContent(),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }
        
        
        [HttpPost("questions")]
        public async Task<IActionResult> CreateQuestion([FromBody] CreateQuestionCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match(
               success => Created(string.Empty, success),
               error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }
        
        
        [HttpPut("questions/{questionId}")]
        public async Task<IActionResult> UpdateQuestion([FromRoute] int questionId, [FromBody] UpdateQuestionCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command with { QuestionId = questionId }, cancellationToken);
            return result.Match(
               success => Ok(success),
               error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }
        
        
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetCategoriesQuery(), cancellationToken));
        }
        
        
        [HttpGet("questions")]
        public async Task<IActionResult> GetQuestions(CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetAllQuestionsQuery(), cancellationToken));
        }
        
        
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics(CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetStatisticsQuery(), cancellationToken));
        }


        [HttpGet("questions/{questionId}")]
        public async Task<IActionResult> GetQuestion([FromRoute] GetQuestionQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return result.Match(
               success => Ok(success),
               error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }
    }
}
