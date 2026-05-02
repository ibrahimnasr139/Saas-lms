using Application.Common;
using Application.Constants;
using Application.Features.Roles.Commands.CreateRole;
using Application.Features.Roles.Commands.DeleteRole;
using Application.Features.Roles.Commands.UpdateRole;
using Application.Features.Roles.Queries.GetTenantRole;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/tenant/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthConstants.ApiScheme)]
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles(CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetTenantRoleQuery(), cancellationToken));
        }


        [HttpPut("{roleId}")]
        public async Task<IActionResult> UpdateRole([FromRoute] int roleId, [FromBody] UpdateRoleCommand updateRoleCommand, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new UpdateRoleCommand(roleId, updateRoleCommand.Name, updateRoleCommand.Description,
                updateRoleCommand.EnabledPermissions), cancellationToken);
            return result.Match(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpDelete("{roleId}")]
        public async Task<IActionResult> DeleteRole([FromRoute] int roleId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteRoleCommand(roleId), cancellationToken);
            return result.Match(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new CreateRoleCommand(request.Name, request.Description, request.EnabledPermissions), cancellationToken);
            return result.Match(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }
    }
}
