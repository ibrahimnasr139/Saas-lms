using Application.Features.TenantPaymentMethods.Dtos;

namespace Application.Features.TenantPaymentMethods.Commands.UpdatePaymentMethodStatus
{
    internal sealed class UpdatePaymentMethodStatusCommandHandler : IRequestHandler<UpdatePaymentMethodStatusCommand, OneOf<PaymentMethodResponse, Error>>
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly ITenantMemberRepository _tenantMemberRepository;

        public UpdatePaymentMethodStatusCommandHandler(IPaymentMethodRepository paymentMethodRepository, ICurrentUserId currentUserId,
            ITenantMemberRepository tenantMemberRepository)
        {
            _paymentMethodRepository = paymentMethodRepository;
            _currentUserId = currentUserId;
            _tenantMemberRepository = tenantMemberRepository;
        }
        public async Task<OneOf<PaymentMethodResponse, Error>> Handle(UpdatePaymentMethodStatusCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var isPermitted = await _tenantMemberRepository.IsPermittedMember(userId, PermissionConstants.MANAGE_INTEGRATIONS, cancellationToken);
            if (!isPermitted)
                return MemberErrors.NotAllowed;

            var updatedPaymentMethodStatus = await _paymentMethodRepository.UpdatePaymentMethodStatusAsync(request.PaymentMethodId, request.IsActive, cancellationToken);
            if (updatedPaymentMethodStatus is null)
                return TenantPaymentMethodErrors.PaymentMethodNotFound;

            return new PaymentMethodResponse
            {
                Data = new PaymentMethodDto
                {
                    Id = updatedPaymentMethodStatus.Id,
                    Type = updatedPaymentMethodStatus.Type,
                    IsActive = updatedPaymentMethodStatus.IsActive,
                    Details = updatedPaymentMethodStatus.Details,
                    CreatedAt = updatedPaymentMethodStatus.CreatedAt
                }
            };
        }
    }
}
