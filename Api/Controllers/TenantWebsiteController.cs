using Application.Common;
using Application.Constants;
using Application.Features.TenantWebsite.Commands.CreateTenantPage;
using Application.Features.TenantWebsite.Commands.DeleteTenantPage;
using Application.Features.TenantWebsite.Commands.DuplicateTenantPage;
using Application.Features.TenantWebsite.Commands.UpdateTenantPage;
using Application.Features.TenantWebsite.Queries.GetTenantPage;
using Application.Features.TenantWebsite.Queries.GetTenantPageBlocks;
using Application.Features.TenantWebsite.Queries.GetTenantPages;
using Application.Features.TenantWebsite.Queries.GetTenantWebsiteCourses;
using Application.Features.TenantWebsite.Queries.ValidateUrl;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/tenant/website")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthConstants.ApiScheme)]

    public class TenantWebsiteController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TenantWebsiteController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("pages")]
        public async Task<IActionResult> GetTenantPages(CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetTenantPagesQuery(), cancellationToken));
        }


        [HttpPost("pages")]
        public async Task<IActionResult> CreateTenantPage([FromBody] CreateTenantPageCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpDelete("pages/{pageId}")]
        public async Task<IActionResult> DeleteTenantPage([FromRoute] int pageId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteTenantPageCommand(pageId), cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("pages/{pageId}/duplicate")]
        public async Task<IActionResult> DuplicateTenantPage([FromRoute] int pageId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DuplicateTenantPageCommand(pageId), cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("pages/blocks")]
        public async Task<IActionResult> GetTenantPageBlocks(CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetTenantPageBlocksQuery(), cancellationToken));
        }


        [HttpGet("pages/validate-url")]
        public async Task<IActionResult> ValidateUrl([FromQuery] string url, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new ValidateUrlQuery(url), cancellationToken));
        }


        [HttpPatch("pages/{pageId}")]
        public async Task<IActionResult> UpdateTenantPage([FromRoute] int pageId, [FromBody] UpdateTenantPageCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new UpdateTenantPageCommand(pageId, command.Title, command.Url, command.Status,
                command.MetaTitle, command.MetaDescription, command.PageBlocks), cancellationToken);

            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("pages/{pageId}")]
        public async Task<IActionResult> GetTenantPage([FromRoute] int pageId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetTenantPageQuery(pageId), cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("courses")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTenantCourses([FromQuery] List<int> courseIds, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetTenantCoursesQuery(courseIds), cancellationToken));
        }
    }
}