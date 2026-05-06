using Application.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services.AuthServices
{
    internal class TokenProvider : ITokenProvider
    {
        private readonly IOptions<JwtOptions> _options;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TokenProvider(IOptions<JwtOptions> options, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _options = options;
        }
        public void GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _options.Value.Issuer,
                audience: _options.Value.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_options.Value.ExpiryMinutes),
                signingCredentials: creds);
            var tokenHandler = new JwtSecurityTokenHandler().WriteToken(token);
            _httpContextAccessor?.HttpContext?.Response.Cookies.Append(AuthConstants.AccessToken, tokenHandler,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    IsEssential = true,
                    Expires = DateTimeOffset.Now.AddMinutes(_options.Value.ExpiryMinutes),
                    // Domain = AuthConstants.CookieDomain
                }
                );
        }

        public (string token, DateTime expiresOn) GenerateRefreshToken()
        {
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var expiresOn = DateTime.UtcNow.AddDays(7);
            _httpContextAccessor?.HttpContext?.Response.Cookies.Append(AuthConstants.RefreshToken, refreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    IsEssential = true,
                    Expires = expiresOn,
                    //  Domain = AuthConstants.CookieDomain
                }
                );
            return (refreshToken, expiresOn);
        }
    }
}
