using Microsoft.AspNetCore.Http;

namespace Application.Features.StudentAuth.Commands.VerifyOtp
{
    internal sealed class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCodeCommand, OneOf<bool, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStudentRepository _studentRepository;

        public VerifyOtpCommandHandler(HybridCache hybridCache, UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor, IStudentRepository studentRepository)
        {
            _hybridCache = hybridCache;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _studentRepository = studentRepository;
        }
        public async Task<OneOf<bool, Error>> Handle(VerifyOtpCodeCommand request, CancellationToken cancellationToken)
        {
            var verificationCode = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.VerificationCode];
            if (verificationCode == null)
                return UserErrors.InvalidVerificationToken;

            var email = await _hybridCache.GetOrCreateAsync<string?>(
                verificationCode,
                _ => ValueTask.FromResult<string?>(null),
                cancellationToken: cancellationToken
            );
            if (email is null)
                return UserErrors.InvalidVerificationToken;

            var cachedOtp = await _hybridCache.GetOrCreateAsync<string?>(
               email,
               _ => ValueTask.FromResult<string?>(null),
               cancellationToken: cancellationToken
            );
            if (cachedOtp is null)
                return UserErrors.InvalidOtpCode;

            if (request.OtpCode != cachedOtp)
                return UserErrors.InvalidOtpCode;

            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return UserErrors.EmailNotFound;

            var studentId = await _studentRepository.GetStudentIdAsync(user.Id, cancellationToken);
            var sessionId = Guid.NewGuid().ToString();
            var session = new UserSession
            {
                UserId = user.Id,
                StudentId = studentId,
                Role = RoleConstants.Student,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(7)
            };

            await _hybridCache.SetAsync(
                $"{CacheKeysConstants.SessionKey}_{sessionId}",
                session,
                new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromDays(7)
                },
                cancellationToken: cancellationToken
            );

            _httpContextAccessor?.HttpContext?.Response.Cookies.Delete(AuthConstants.VerificationCode);
            await _hybridCache.RemoveAsync(email, cancellationToken);
            await _hybridCache.RemoveAsync(verificationCode, cancellationToken);

            _httpContextAccessor?.HttpContext?.Response.Cookies.Append(
                AuthConstants.SessionId,
                sessionId,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    Domain = AuthConstants.CookieDomain
                }
            );
            return true;
        }
    }
}