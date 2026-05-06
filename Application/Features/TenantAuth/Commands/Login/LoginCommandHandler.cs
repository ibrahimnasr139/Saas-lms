using Application.Contracts.Authentication;
using Application.Helpers;
using Hangfire;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TenantAuth.Commands.Login
{
    internal sealed class LoginCommandHandler : IRequestHandler<LoginCommand, OneOf<LoginDto, Error>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRefreshRepository _refreshRepository;
        private readonly IMapper _mapper;
        private readonly ITokenProvider _tokenProvider;
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        public LoginCommandHandler(UserManager<ApplicationUser> userManager, IRefreshRepository refreshRepository, IMapper mapper,
            ITokenProvider tokenProvider, HybridCache hybridCache, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _refreshRepository = refreshRepository;
            _mapper = mapper;
            _tokenProvider = tokenProvider;
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }

        public async Task<OneOf<LoginDto, Error>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return UserErrors.InvalidCredentials;

            var signInResult = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!signInResult)
                return UserErrors.InvalidCredentials;

            if (!user.EmailConfirmed)
            {
                var otpCode = await GenerateOtpHelper.GenerateOtp(request.Email, _hybridCache, _httpContextAccessor, cancellationToken);
                var emailBody = EmailConfirmationHelper.GenerateEmailBodyHelper(EmailConstants.OtpTemplate, new Dictionary<string, string>
                {
                    { "{{OTP_CODE}}", otpCode },
                    { "{{UserName}}", user.FirstName }
                });
                BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(request.Email, EmailConstants.EmailConfirmationSubject, emailBody));
                return UserErrors.EmailNotConfirmed;
            }
            if (user.LastActiveTenantSubDomain is not null)
            {
                _httpContextAccessor?.HttpContext?.Response.Cookies.Append(AuthConstants.SubDomain, user.LastActiveTenantSubDomain, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    // Domain = AuthConstants.CookieDomain,
                    IsEssential = true
                });
            }
            _tokenProvider.GenerateJwtToken(user!);
            var refreshToken = _tokenProvider.GenerateRefreshToken();
            _refreshRepository.AddRefreshToken(user, refreshToken.token, refreshToken.expiresOn, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            return _mapper.Map<LoginDto>(user);
        }
    }
}
