using Application.Common;
using Application.Constants;
using Application.Features.TenantMembers.Commands.AcceptTenanInvite;
using Application.Features.TenantMembers.Commands.DeclineTenanInvite;
using Application.Features.TenantMembers.Commands.InviteTenantMember;
using Application.Features.TenantMembers.Commands.RemoveMember;
using Application.Features.TenantMembers.Commands.UpdateCurrentMember;
using Application.Features.TenantMembers.Commands.UpdateMemberRole;
using Application.Features.TenantMembers.Commands.ValidateTenanInvite;
using Application.Features.TenantMembers.Queries.GetCurrentTenantMember;
using Application.Features.TenantMembers.Queries.GetMemberProfile;
using Application.Features.TenantMembers.Queries.GetTenantMembers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/tenant/[Controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthConstants.ApiScheme)]
    public class MembersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MembersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetTenantMembers(CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetTenantMembersQuery(), cancellationToken));
        }


        [HttpGet("current")]
        public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetCurrentTenantMemberQuery(), cancellationToken));
        }


        [HttpPost("invite")]
        public async Task<IActionResult> Invite([FromBody] InviteTenantMemberCommand inviteTenantMemberCommand, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(inviteTenantMemberCommand, cancellationToken);
            return result.Match(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("invite/validate")]
        public async Task<IActionResult> Create([FromQuery] string token, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new ValidateTenanInviteCommand(token), cancellationToken));
        }


        [HttpPost("invite/accept")]
        public async Task<IActionResult> AcceptInvite([FromQuery] string token, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new AcceptTenanInviteCommand(token), cancellationToken);
            return result.Match(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("invite/decline")]
        public async Task<IActionResult> DeclineInvite([FromQuery] string token, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new DeclineTenanInviteCommand(token), cancellationToken));
        }


        [HttpDelete("{memberId}")]
        public async Task<IActionResult> RemoveMember([FromRoute] int memberId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RemoveMemberCommand(memberId), cancellationToken);
            return result.Match(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPatch("{memberId}/role")]
        public async Task<IActionResult> UpdateMemberRole([FromRoute] int memberId, [FromBody] int roleId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new UpdateMemberRoleCommand(memberId, roleId), cancellationToken);
            return result.Match(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("{memberId}/profile")]
        public async Task<IActionResult> GetMemberProfile([FromRoute] int memberId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetMemberProfileQuery(memberId), cancellationToken);
            return result.Match(
               success => Ok(success),
               error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
           );
        }


        [HttpPatch("current")]
        public async Task<IActionResult> UpdateCurrentMember([FromBody] UpdateCurrentMemberCommand updateCurrentMemberCommand, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(updateCurrentMemberCommand, cancellationToken));
        }
    }
}
