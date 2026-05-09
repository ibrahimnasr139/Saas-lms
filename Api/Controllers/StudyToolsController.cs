using Application.Common;
using Application.Features.StudyTools.Commands.AskAi;
using Application.Features.StudyTools.Commands.AttemptQuiz;
using Application.Features.StudyTools.Commands.CreateFlashCardDeck;
using Application.Features.StudyTools.Commands.CreateQuiz;
using Application.Features.StudyTools.Commands.ReviewFlashCard;
using Application.Features.StudyTools.Queries.GetFlashCardDeckDetails;
using Application.Features.StudyTools.Queries.GetFlashCardDecks;
using Application.Features.StudyTools.Queries.GetStudentQuiz;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/student")]
    [ApiController]
    [Authorize]
    public class StudyToolsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StudyToolsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("ask-ai")]
        public async Task<IActionResult> AskAi([FromBody] AskAiCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                response => Ok(response),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("decks")]
        public async Task<IActionResult> GetFlashCardDecks(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetFlashCardDecksQuery(), cancellationToken);
            return result.Match<IActionResult>(
                response => Ok(response),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("decks")]
        public async Task<IActionResult> CreateFlashCardDeck([FromBody] CreateFlashCardDeckCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                response => Ok(response),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("decks/{deckId}")]
        public async Task<IActionResult> GetFlashCardDeckDetails([FromRoute] GetFlashCardDeckDetailsQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return result.Match<IActionResult>(
                response => Ok(response),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("decks/{deckId}/flashcards/{flashcardId}/review")]
        public async Task<IActionResult> ReviewFlashCard([FromRoute] int deckId, [FromRoute] int flashcardId, [FromBody] ReviewFlashCardCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command with { DeckId = deckId, FlashCardId = flashcardId }, cancellationToken);
            return result.Match<IActionResult>(
                response => Ok(response),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("quizzes")]
        public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                response => Ok(response),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("quizzes/{quizId}")]
        public async Task<IActionResult> GetQuiz([FromRoute] GetStudentQuizQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return result.Match<IActionResult>(
                response => Ok(response),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("quizzes/{quizId}/attempts")]
        public async Task<IActionResult> AttemptQuiz([FromRoute] int quizId, [FromBody] AttemptQuizQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query with { QuizId = quizId }, cancellationToken);
            return result.Match<IActionResult>(
                response => Ok(response),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }
    }
}