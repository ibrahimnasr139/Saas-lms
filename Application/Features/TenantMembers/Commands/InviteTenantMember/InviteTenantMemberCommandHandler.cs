using Application.Features.TenantMembers.Dtos;
using Application.Helpers;
using Hangfire;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TenantMembers.Commands.InviteTenantMember
{
    internal sealed class InviteTenantMemberCommandHandler : IRequestHandler<InviteTenantMemberCommand, OneOf<InviteTenantMemberDto, Error>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantInviteRepository _tenantInviteRepository;
        private readonly ITenantRoleRepository _tenantRoleRepository;
        private readonly IEmailSender _emailSender;
        private readonly ICurrentUserId _currentUserId;

        public InviteTenantMemberCommandHandler(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork,
            ITenantMemberRepository tenantMemberRepository, ITenantRepository tenantRepository,
            IHttpContextAccessor httpContextAccessor, ITenantInviteRepository tenantInviteRepository,
            ITenantRoleRepository tenantRoleRepository, IEmailSender emailSender, ICurrentUserId currentUserId)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _tenantMemberRepository = tenantMemberRepository;
            _tenantRepository = tenantRepository;
            _httpContextAccessor = httpContextAccessor;
            _tenantInviteRepository = tenantInviteRepository;
            _tenantRoleRepository = tenantRoleRepository;
            _emailSender = emailSender;
            _currentUserId = currentUserId;
        }
        public async Task<OneOf<InviteTenantMemberDto, Error>> Handle(InviteTenantMemberCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserId.GetUserId();
            var isPermitted = await _tenantMemberRepository.IsPermittedMember(currentUserId, PermissionConstants.INVITE_MEMBERS, cancellationToken);
            if (!isPermitted)
                return MemberErrors.NotAllowed;

            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);
            var user = await _userManager.FindByEmailAsync(request.email);

            if (user is not null)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var TenantIds = await _tenantMemberRepository.GetTenantIdsAsync(userId, cancellationToken);
                if (TenantIds.Contains(tenantId))
                    return TenantInviteErrors.UserAlreadyExists;
            }

            var invitedByMemberId = await _tenantMemberRepository.GetMemberIdByUserIdAsync(currentUserId, tenantId, cancellationToken);

            var tenant = await _tenantRepository.GetLastTenantAsync(subDomain, cancellationToken);
            var roleName = await _tenantRoleRepository.GetRoleNameAsync(request.roleId, cancellationToken);

            var tenantInvite = new TenantInvite
            {
                Email = request.email,
                TenantId = tenantId,
                TenantRoleId = request.roleId,
                InvitedBy = invitedByMemberId
            };
            await _tenantInviteRepository.CreateTenantInviteAsync(tenantInvite, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);

            var emailBody = EmailConfirmationHelper.GenerateEmailBodyHelper(
                EmailConstants.TenantInviteTemplate,
                new Dictionary<string, string>
                {
                    { "{{platform_name}}", tenant?.PlatformName! },
                    { "{{role_name}}", roleName! },
                    { "{{action_url}}", $"{EmailConstants.TenantMemberInviteUrl}?token={tenantInvite.Token}" },
                    { "{{expiry_hours}}", "24" },
                    { "{{year}}", DateTime.UtcNow.Year.ToString() }
                });

            BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(request.email, EmailConstants.Subject, emailBody));

            return new InviteTenantMemberDto { Message = $"{MessagesConstants.TenantInviteSent} {request.email}" };
        }
    }
}
