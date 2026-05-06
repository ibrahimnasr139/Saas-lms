using Microsoft.AspNetCore.Http;

namespace Application.Helpers
{
    public static class GenerateOtpHelper
    {
        public static async Task<string> GenerateOtp(string email, HybridCache hybridCache,
            IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken)
        {
            var otpCode = new Random().Next(100000, 999999).ToString();
            var verificationCode = Guid.NewGuid().ToString();
            await hybridCache.SetAsync(verificationCode, email, cancellationToken: cancellationToken);
            await hybridCache.SetAsync(email, otpCode, new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(2)
            }, cancellationToken: cancellationToken);
            httpContextAccessor?.HttpContext?.Response.Cookies.Append(AuthConstants.VerificationCode, verificationCode, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                IsEssential = true,
                Domain = AuthConstants.CookieDomain
            });
            return otpCode;
        }
    }
}
