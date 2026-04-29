using Application.Features.TenantMembers.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TenantMembers.Commands.AcceptTenanInvite
{
    internal sealed class AcceptTenanInviteCommandHandler : IRequestHandler<AcceptTenanInviteCommand, OneOf<AcceptTenanInviteDto, Error>>
    {
        private readonly ICurrentUserId _currentUserId;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITenantInviteRepository _tenantInviteRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AcceptTenanInviteCommandHandler(ICurrentUserId currentUserId, UserManager<ApplicationUser> userManager,
            ITenantInviteRepository tenantInviteRepository, ITenantRepository tenantRepository, IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor)
        {
            _currentUserId = currentUserId;
            _userManager = userManager;
            _tenantInviteRepository = tenantInviteRepository;
            _tenantRepository = tenantRepository;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<OneOf<AcceptTenanInviteDto, Error>> Handle(AcceptTenanInviteCommand request, CancellationToken cancellationToken)
        {
            var isValidToken = await _tenantInviteRepository.IsValidTokenAsync(request.token, cancellationToken);
            if (!isValidToken)
                return TenantInviteErrors.InviteExpired;

            var userId = _currentUserId.GetUserId();
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return TenantInviteErrors.InviteError;

            var userEmail = await _userManager.GetEmailAsync(user);
            var invitedEmail = await _tenantInviteRepository.GetInvitedMemberEmailAsync(request.token, cancellationToken);

            if (!string.Equals(userEmail, invitedEmail))
                return TenantInviteErrors.InviteError;

            var invite = await _tenantInviteRepository.GetInviteByTokenAsync(request.token, cancellationToken);
            if (invite == null)
                return TenantInviteErrors.InviteNotFound;

            var tenantMember = new TenantMember
            {
                TenantId = invite.TenantId,
                UserId = userId,
                TenantRoleId = invite.TenantRoleId,
                InvitedById = invite.InvitedBy,
                JobTitle = "-",
                DisplayName = $"{user.FirstName} {user.LastName}"
            };

            await _tenantRepository.AddTenantMemberAsync(tenantMember, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            await _tenantInviteRepository.AcceptInviteAsync(request.token, cancellationToken);

            var subdomain = await _tenantRepository.GetSubDomainAsync(invite.TenantId, cancellationToken);
            _httpContextAccessor?.HttpContext?.Response.Cookies.Append(AuthConstants.SubDomain, subdomain, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Domain = AuthConstants.CookieDomain,
                IsEssential = true
            });
            return new AcceptTenanInviteDto { Message = MessagesConstants.TenantInviteAccepted, Subdomain = subdomain };
        }
    }
}
