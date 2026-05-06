using Application.Features.TenantMembers.Dtos;

namespace Application.Features.TenantMembers.Commands.ValidateTenanInvite
{
    internal sealed class ValidateTenanInviteCommandHandler : IRequestHandler<ValidateTenanInviteCommand, ValidateTenanInviteDto>
    {
        private readonly ITenantInviteRepository _tenantInviteRepository;

        public ValidateTenanInviteCommandHandler(ITenantInviteRepository tenantInviteRepository)
        {
            _tenantInviteRepository = tenantInviteRepository;
        }
        public Task<ValidateTenanInviteDto> Handle(ValidateTenanInviteCommand request, CancellationToken cancellationToken) =>
            _tenantInviteRepository.GetValidateTenanInviteAsync(request.Token, cancellationToken);
    }
}
