using Application.Features.TenantMembers.Dtos;

namespace Application.Features.TenantMembers.Commands.DeclineTenanInvite
{
    internal sealed class DeclineTenanInviteCommandHandler : IRequestHandler<DeclineTenanInviteCommand, DeclineTenanInviteDto>
    {
        private readonly ITenantInviteRepository _tenantInviteRepository;

        public DeclineTenanInviteCommandHandler(ITenantInviteRepository tenantInviteRepository)
        {
            _tenantInviteRepository = tenantInviteRepository;
        }
        public async Task<DeclineTenanInviteDto> Handle(DeclineTenanInviteCommand request, CancellationToken cancellationToken)
        {
            await _tenantInviteRepository.DeclineInviteAsync(request.token, cancellationToken);
            return new DeclineTenanInviteDto { Message = MessagesConstants.TenantInviteDeclined };
        }
    }
}
