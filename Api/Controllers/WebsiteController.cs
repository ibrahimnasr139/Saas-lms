using Application.Common;
using Application.Constants;
using Application.Features.Website.Commands.AddPaymentMethod;
using Application.Features.Website.Commands.ApproveOrder;
using Application.Features.Website.Commands.BulkOrderAction;
using Application.Features.Website.Commands.CreateTenantPage;
using Application.Features.Website.Commands.DeclineOrder;
using Application.Features.Website.Commands.DeletePaymentMethod;
using Application.Features.Website.Commands.DeleteTenantPage;
using Application.Features.Website.Commands.DuplicateTenantPage;
using Application.Features.Website.Commands.UpdatePaymentMethod;
using Application.Features.Website.Commands.UpdatePaymentMethodStatus;
using Application.Features.Website.Commands.UpdateTenantPage;
using Application.Features.Website.Commands.UpdateTenantWebsiteSettings;
using Application.Features.Website.Queries.GetPaymentMethods;
using Application.Features.Website.Queries.GetSettings;
using Application.Features.Website.Queries.GetStatistics;
using Application.Features.Website.Queries.GetTenantOrders;
using Application.Features.Website.Queries.GetTenantOrderStatistics;
using Application.Features.Website.Queries.GetTenantPage;
using Application.Features.Website.Queries.GetTenantPageBlocks;
using Application.Features.Website.Queries.GetTenantPages;
using Application.Features.Website.Queries.ValidateUrl;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/tenant/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthConstants.ApiScheme)]
    public class WebsiteController : ControllerBase
    {
        private readonly IMediator _mediator;
        public WebsiteController(IMediator mediator)
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


        [HttpGet("settings")]
        public async Task<IActionResult> GetSettings(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetTenantWebsiteSettingsQuery(), cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPatch("settings")]
        public async Task<IActionResult> UpdateSettings([FromBody] UpdateTenantWebsiteSettingsCommand request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("payment-methods")]
        public async Task<IActionResult> GetPaymentMethods(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetPaymentMethodsQuery(), cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("payment-methods")]
        public async Task<IActionResult> AddPaymentMethod([FromBody] AddPaymentMethodCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPatch("payment-methods/{paymentMethodId}")]
        public async Task<IActionResult> UpdatePaymentMethod([FromRoute] int paymentMethodId, [FromBody] UpdatePaymentMethodCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command with { PaymentMethodId = paymentMethodId }, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPatch("payment-methods/{paymentMethodId}/status")]
        public async Task<IActionResult> UpdatePaymentMethodStatus([FromRoute] int paymentMethodId, [FromBody] UpdatePaymentMethodStatusCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command with { PaymentMethodId = paymentMethodId }, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpDelete("payment-methods/{paymentMethodId}")]
        public async Task<IActionResult> DeletePaymentMethod([FromRoute] int paymentMethodId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeletePaymentMethodCommand(paymentMethodId), cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders(CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetTenantOrdersQuery(), cancellationToken));
        }


        [HttpGet("orders/statistics")]
        public async Task<IActionResult> GetOrderStatistics(CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetTenantOrderStatisticsQuery(), cancellationToken));
        }


        [HttpPost("orders/{orderId}/approve")]
        public async Task<IActionResult> ApproveOrder([FromRoute] int orderId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ApproveOrderCommand(orderId), cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("orders/{orderId}/decline")]
        public async Task<IActionResult> DeclineOrder([FromRoute] int orderId, [FromBody] DeclineOrderCommand? command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeclineOrderCommand(orderId, command?.Reason), cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("orders/bulk-action")]
        public async Task<IActionResult> BulkAction([FromBody] BulkOrderActionCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new BulkOrderActionCommand(command.OrderIds, command.Action, command.Reason), cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetWebsiteStatisticsQuery(), cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }
    }
}