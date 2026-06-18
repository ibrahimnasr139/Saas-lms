using Application.Features.Website.Dtos;

namespace Application.Features.Website.Commands.UpdatePaymentMethod
{
    internal sealed class UpdatePaymentMethodCommandHandler : IRequestHandler<UpdatePaymentMethodCommand, OneOf<PaymentMethodResponse, Error>>
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly ITenantMemberRepository _tenantMemberRepository;

        public UpdatePaymentMethodCommandHandler(IPaymentMethodRepository paymentMethodRepository, ICurrentUserId currentUserId,
            ITenantMemberRepository tenantMemberRepository)
        {
            _paymentMethodRepository = paymentMethodRepository;
            _currentUserId = currentUserId;
            _tenantMemberRepository = tenantMemberRepository;
        }
        public async Task<OneOf<PaymentMethodResponse, Error>> Handle(UpdatePaymentMethodCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var isPermitted = await _tenantMemberRepository.IsPermittedMember(userId, PermissionConstants.MANAGE_INTEGRATIONS, cancellationToken);
            if (!isPermitted)
                return MemberErrors.NotAllowed;

            var updatePaymentMethod = await _paymentMethodRepository.UpdatePaymentMethodAsync(request.PaymentMethodId, request.Details, cancellationToken);
            if (updatePaymentMethod is null)
                return TenantPaymentMethodErrors.PaymentMethodNotFound;

            return new PaymentMethodResponse
            {
                Data = new PaymentMethodDto
                {
                    Id = updatePaymentMethod.Id,
                    Type = updatePaymentMethod.Type,
                    IsActive = updatePaymentMethod.IsActive,
                    Details = updatePaymentMethod.Details
                }
            };
        }
    }
}
