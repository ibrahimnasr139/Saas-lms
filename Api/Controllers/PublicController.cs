using Application.Common;
using Application.Features.Public.Commands.CreateOrder;
using Application.Features.Public.Commands.UpdateReceipt;
using Application.Features.Public.Queries.GetCourseDetails;
using Application.Features.Public.Queries.GetOrder;
using Application.Features.Public.Queries.GetTenantNavigationLinks;
using Application.Features.Public.Queries.GetTenantPages;
using Application.Features.Public.Queries.GetTenantPaymentMethods;
using Application.Features.Public.Queries.GetTenantSettings;
using Application.Features.Public.Queries.GetTenantWebsiteCourses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/tenant/website/[controller]")]
    [ApiController]
    public class PublicController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PublicController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("pages/navigation")]
        public async Task<IActionResult> GetTenantNavigationLinks(CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetTenantNavigationLinksQuery(), cancellationToken));
        }


        [HttpGet("pages")]
        public async Task<IActionResult> GetTenantPages([FromQuery] string url, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetTenantPagesQuery(url), cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("settings")]
        public async Task<IActionResult> GetTenantSettings(CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetTenantSettingsQuery(), cancellationToken));
        }


        [HttpGet("courses/{courseId}")]
        public async Task<IActionResult> GetCourseDetails([FromRoute] int courseId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCourseDetailsQuery(courseId), cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("courses")]
        public async Task<IActionResult> GetTenantCourses([FromQuery] List<int> courseIds, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetTenantCoursesQuery(courseIds), cancellationToken));
        }


        [Authorize]
        [HttpGet("payment-methods")]
        public async Task<IActionResult> GetPaymentMethods(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetTenantPaymentMethodsQuery(), cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [Authorize]
        [HttpPost("orders")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [Authorize]
        [HttpGet("orders/{orderId}")]
        public async Task<IActionResult> GetOrder([FromRoute] GetOrderQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [Authorize]
        [HttpPatch("orders/{orderId}/receipt")]
        public async Task<IActionResult> UpdateOrderReceipt([FromRoute] int orderId, [FromBody] UpdateReceiptCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command with { OrderId = orderId }, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }
    }
}