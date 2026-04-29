using Application.Contracts.Repositories;
using Application.Features.TenantPaymentMethods.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TenantPaymentMethods.Commands.AddPaymentMethod
{
    internal sealed class AddPaymentMethodCommandHandler : IRequestHandler<AddPaymentMethodCommand, OneOf<PaymentMethodResponse, Error>>
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantRepository _tenantRepository;

        public AddPaymentMethodCommandHandler(IPaymentMethodRepository paymentMethodRepository, ICurrentUserId currentUserId,
            ITenantMemberRepository tenantMemberRepository, IHttpContextAccessor httpContextAccessor,IUnitOfWork unitOfWork,
            ITenantRepository tenantRepository)
        {
            _paymentMethodRepository = paymentMethodRepository;
            _currentUserId = currentUserId;
            _tenantMemberRepository = tenantMemberRepository;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
            _tenantRepository = tenantRepository;
        }
        public async Task<OneOf<PaymentMethodResponse, Error>> Handle(AddPaymentMethodCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var isPermitted = await _tenantMemberRepository.IsPermittedMember(userId, PermissionConstants.MANAGE_INTEGRATIONS, cancellationToken);
            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);
            if (!isPermitted)
                return MemberErrors.NotAllowed;
            
            var existingPaymentMethods = await _paymentMethodRepository.IsPaymentMethodTypeExistAsync(tenantId, request.Type, cancellationToken);
            if(existingPaymentMethods)
                return TenantPaymentMethodErrors.AlreadyExists;

            var paymentMethod = new PaymentMethod
            {
                Details = request.Details,
                Type = request.Type,
                TenantId = tenantId
            };

            await _paymentMethodRepository.CreatePaymentMethodAsync(paymentMethod, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);

            if (paymentMethod.Id == 0)
                return TenantPaymentMethodErrors.CreateFailed;

            return new PaymentMethodResponse
            {
                Data = new PaymentMethodDto
                {
                    Id = paymentMethod.Id,
                    Details = paymentMethod.Details,
                    Type = paymentMethod.Type,
                    IsActive = paymentMethod.IsActive
                }
            };
        }
    }
}
