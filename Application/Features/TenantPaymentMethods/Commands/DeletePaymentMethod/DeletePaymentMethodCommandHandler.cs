using Application.Features.TenantPaymentMethods.Dtos;

namespace Application.Features.TenantPaymentMethods.Commands.DeletePaymentMethod
{
    internal sealed class DeletePaymentMethodCommandHandler : IRequestHandler<DeletePaymentMethodCommand, OneOf<PaymentMethodResponseMessage, Error>>
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly ITenantMemberRepository _tenantMemberRepository;

        public DeletePaymentMethodCommandHandler(IPaymentMethodRepository paymentMethodRepository, ITenantRepository tenantRepository,
            ICurrentUserId currentUserId, ITenantMemberRepository tenantMemberRepository)
        {
            _paymentMethodRepository = paymentMethodRepository;
            _tenantRepository = tenantRepository;
            _currentUserId = currentUserId;
            _tenantMemberRepository = tenantMemberRepository;
        }
        public async Task<OneOf<PaymentMethodResponseMessage, Error>> Handle(DeletePaymentMethodCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var isPermitted = await _tenantMemberRepository.IsPermittedMember(userId, PermissionConstants.MANAGE_INTEGRATIONS, cancellationToken);
            if (!isPermitted)
                return MemberErrors.NotAllowed;

            var deletePaymentMethod = await _paymentMethodRepository.DeletePayMentMethodAsync(request.paymentMethodId, cancellationToken);
            if (!deletePaymentMethod)
                return TenantPaymentMethodErrors.DeleteFailed;

            return new PaymentMethodResponseMessage { Message = MessagesConstants.TenantPaymentMethodsDeleted };
        }
    }
}