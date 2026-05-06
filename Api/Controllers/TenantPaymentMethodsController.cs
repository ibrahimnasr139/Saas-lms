using Application.Common;
using Application.Constants;
using Application.Features.TenantPaymentMethods.Commands.AddPaymentMethod;
using Application.Features.TenantPaymentMethods.Commands.DeletePaymentMethod;
using Application.Features.TenantPaymentMethods.Commands.UpdatePaymentMethod;
using Application.Features.TenantPaymentMethods.Commands.UpdatePaymentMethodStatus;
using Application.Features.TenantPaymentMethods.Queries.GetPaymentMethods;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/tenant/website/payment-methods")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthConstants.ApiScheme)]
    public class TenantPaymentMethodsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TenantPaymentMethodsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaymentMethods(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetPaymentMethodsQuery(), cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost]
        public async Task<IActionResult> AddPaymentMethod([FromBody] AddPaymentMethodCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPatch("{paymentMethodId}")]
        public async Task<IActionResult> UpdatePaymentMethod([FromRoute] int paymentMethodId, [FromBody] UpdatePaymentMethodCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command with { PaymentMethodId = paymentMethodId }, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPatch("{paymentMethodId}/status")]
        public async Task<IActionResult> UpdatePaymentMethodStatus([FromRoute] int paymentMethodId, [FromBody] UpdatePaymentMethodStatusCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command with { PaymentMethodId = paymentMethodId }, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpDelete("{paymentMethodId}")]
        public async Task<IActionResult> DeletePaymentMethod([FromRoute] int paymentMethodId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeletePaymentMethodCommand(paymentMethodId), cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }
    }
}