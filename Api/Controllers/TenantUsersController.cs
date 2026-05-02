using Application.Constants;
using Application.Features.TenantUsers.Queries.GetProfile;
using Application.Features.TenantUsers.Queries.GetTenants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/tenant/users/me")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthConstants.ApiScheme)]
    public class TenantUsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TenantUsersController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetProfileQuery(), cancellationToken));
        }


        [HttpGet("tenants")]
        public async Task<IActionResult> GetTenants(CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetTenantsQuery(), cancellationToken));
        }
    }
}
