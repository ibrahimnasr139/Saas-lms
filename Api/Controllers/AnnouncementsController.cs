using Application.Common;
using Application.Constants;
using Application.Features.Announcements.Commands.CreateAnnouncement;
using Application.Features.Announcements.Commands.DeleteAnnouncement;
using Application.Features.Announcements.Queries.GetAnnouncements;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/tenant/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthConstants.ApiScheme)]
    public class AnnouncementsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AnnouncementsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet()]
        public async Task<IActionResult> GetAnnouncements([FromQuery] GetAnnouncementsQuery query, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(query, cancellationToken));
        }


        [HttpPost()]
        public async Task<IActionResult> CreateAnnouncement([FromBody] CreateAnnouncementCommand command, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(command, cancellationToken));
        }


        [HttpDelete("{announcementId}")]
        public async Task<IActionResult> DeleteAnnouncement([FromRoute] DeleteAnnouncementCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                announcementResponse => Ok(announcementResponse),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }
    }
}