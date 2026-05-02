using Application.Common;
using Application.Constants;
using Application.Features.TenantOrders.Commands.ApproveOrder;
using Application.Features.TenantOrders.Commands.BulkOrderAction;
using Application.Features.TenantOrders.Commands.DeclineOrder;
using Application.Features.TenantOrders.Queries.GetTenantOrders;
using Application.Features.TenantOrders.Queries.GetTenantOrderStatistics;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/tenant/website/orders")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthConstants.ApiScheme)]
    public class TenantOrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TenantOrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders(CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetTenantOrdersQuery(), cancellationToken));
        }


        [HttpGet("statistics")]
        public async Task<IActionResult> GetOrderStatistics(CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetTenantOrderStatisticsQuery(), cancellationToken));
        }


        [HttpPost("{orderId}/approve")]
        public async Task<IActionResult> ApproveOrder([FromRoute] int orderId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ApproveOrderCommand(orderId), cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("{orderId}/decline")]
        public async Task<IActionResult> DeclineOrder([FromRoute] int orderId, [FromBody] DeclineOrderCommand? command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeclineOrderCommand(orderId, command?.Reason), cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("bulk-action")]
        public async Task<IActionResult> BulkAction([FromBody] BulkOrderActionCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new BulkOrderActionCommand(command.OrderIds, command.Action, command.Reason), cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }
    }
}
