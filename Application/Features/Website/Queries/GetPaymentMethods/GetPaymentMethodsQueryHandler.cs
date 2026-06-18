using Application.Features.Website.Dtos;

namespace Application.Features.Website.Queries.GetPaymentMethods
{
    internal sealed class GetPaymentMethodsQueryHandler : IRequestHandler<GetPaymentMethodsQuery, OneOf<List<PaymentMethodDto>, Error>>
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly ITenantMemberRepository _tenantMemberRepository;

        public GetPaymentMethodsQueryHandler(IPaymentMethodRepository paymentMethodRepository, ICurrentUserId currentUserId,
            ITenantMemberRepository tenantMemberRepository)
        {
            _paymentMethodRepository = paymentMethodRepository;
            _currentUserId = currentUserId;
            _tenantMemberRepository = tenantMemberRepository;
        }
        public async Task<OneOf<List<PaymentMethodDto>, Error>> Handle(GetPaymentMethodsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var isPermitted = await _tenantMemberRepository.IsPermittedMember(userId, PermissionConstants.MANAGE_INTEGRATIONS, cancellationToken);
            if (!isPermitted)
                return MemberErrors.NotAllowed;
            return await _paymentMethodRepository.GetPaymentMethodsAsync(cancellationToken);
        }
    }
}
