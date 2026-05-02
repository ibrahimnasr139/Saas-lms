using Application.Common;
using Application.Constants;
using Application.Features.TenantWebsiteSettings.Commands.UpdateTenantWebsiteSettings;
using Application.Features.TenantWebsiteSettings.Queries.GetSettings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/tenant/website/settings")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthConstants.ApiScheme)]
    public class TenantWebsiteSettingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TenantWebsiteSettingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetSettings(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetTenantWebsiteSettingsQuery(), cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPatch]
        public async Task<IActionResult> UpdateSettings([FromBody] UpdateTenantWebsiteSettingsCommand request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }
    }
}