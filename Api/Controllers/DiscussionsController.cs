using Application.Common;
using Application.Constants;
using Application.Features.Discussions.Commands.CreateDiscussionThreadRead;
using Application.Features.Discussions.Commands.DeleteDiscussionReply;
using Application.Features.Discussions.Commands.DeleteDiscussionThread;
using Application.Features.Discussions.Commands.UpdateDiscussionReply;
using Application.Features.Discussions.Queries.GetAllDiscussions;
using Application.Features.Discussions.Queries.GetDiscussionReplies;
using Application.Features.Discussions.Queries.GetDiscussionStatistics;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/tenant/[controller]/threads")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthConstants.ApiScheme)]
    public class DiscussionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DiscussionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetDiscussions([FromQuery] GetAllDiscussionsQuery Query, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(Query, cancellationToken));
        }


        [HttpGet("{threadId}/replies")]
        public async Task<IActionResult> GetDiscussionReplies([FromRoute] GetDiscussionRepliesQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return result.Match<IActionResult>(
               discussions => Ok(discussions),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("statistics")]
        public async Task<IActionResult> GetDiscussionStatistics(CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetDiscussionStatisticsQuery(), cancellationToken));
        }


        [HttpPost("{threadId}/read")]
        public async Task<IActionResult> CreateDiscussionThreadRead([FromRoute] CreateDiscussionThreadReadCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
               success => Ok(new { Message = "Success" }),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpDelete("{threadId}")]
        public async Task<IActionResult> DeleteDiscussionThread([FromRoute] DeleteDiscussionThreadCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
               success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpDelete("{threadId}/replies/{replyId}")]
        public async Task<IActionResult> DeleteDiscussionReply([FromRoute] DeleteDiscussionReplyCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
               success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPut("{threadId}/replies/{replyId}")]
        public async Task<IActionResult> UpdateDiscussionReply([FromRoute] int threadId, [FromRoute] int replyId, [FromBody] UpdateDiscussionReplyCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command with { ThreadId = threadId, ReplyId = replyId }, cancellationToken);
            return result.Match<IActionResult>(
               success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }
    }
}