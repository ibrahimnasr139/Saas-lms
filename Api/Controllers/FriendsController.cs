using Application.Common;
using Application.Features.Friends.Commands.AcceptRequest;
using Application.Features.Friends.Commands.RejectRequest;
using Application.Features.Friends.Commands.SendRequest;
using Application.Features.Friends.Queries.GetFriends;
using Application.Features.Friends.Queries.GetRequests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/student/[controller]")]
    [ApiController]
    [Authorize]
    public class FriendsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public FriendsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetFriends(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetStudentFriendsQuery(), cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("requests")]
        public async Task<IActionResult> GetRequests(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetRequestsQuery(), cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("requests")]
        public async Task<IActionResult> SendRequest([FromBody] SendRequestCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("requests/{requestId}/accept")]
        public async Task<IActionResult> AcceptRequest([FromRoute] int requestId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new AcceptRequestCommand(requestId), cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("requests/{requestId}/reject")]
        public async Task<IActionResult> RejectRequest([FromRoute] int requestId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RejectRequestCommand(requestId), cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }
    }
}