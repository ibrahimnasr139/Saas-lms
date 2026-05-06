using Application.Contracts.Authentication;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TenantAuth.Commands.VerifyOtp
{
    internal sealed class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, OneOf<bool, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRefreshRepository _refreshRepository;
        private readonly ITokenProvider _tokenProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public VerifyOtpCommandHandler(HybridCache hybridCache, UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork,
            IRefreshRepository refreshRepository, ITokenProvider tokenProvider, IHttpContextAccessor httpContextAccessor)
        {
            _hybridCache = hybridCache;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _refreshRepository = refreshRepository;
            _tokenProvider = tokenProvider;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<OneOf<bool, Error>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            var verificationCode = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.VerificationCode];
            if (verificationCode == null)
                return UserErrors.InvalidVerificationToken;

            var email = await _hybridCache.GetOrCreateAsync(verificationCode, async entry =>
            {
                return await Task.FromResult<string?>(null);
            }, cancellationToken: cancellationToken);

            if (email is null)
                return UserErrors.InvalidVerificationToken;

            if (request.OtpCode != await _hybridCache.GetOrCreateAsync(email, async entry =>
            {
                return await Task.FromResult<string?>(null);
            }, cancellationToken: cancellationToken))
            {
                return UserErrors.InvalidOtpCode;
            }
            _httpContextAccessor?.HttpContext?.Response.Cookies.Delete(AuthConstants.VerificationCode);
            await _hybridCache.RemoveAsync(email);
            await _hybridCache.RemoveAsync(verificationCode);
            var user = await _userManager.FindByEmailAsync(email);
            user?.EmailConfirmed = true;
            _tokenProvider.GenerateJwtToken(user!);
            var refreshToken = _tokenProvider.GenerateRefreshToken();
            _refreshRepository.AddRefreshToken(user!, refreshToken.token, refreshToken.expiresOn, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            return true;
        }
    }
}
