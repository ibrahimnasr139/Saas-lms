using Application.Contracts.Authentication;

namespace Application.Features.TenantAuth.Commands.Refresh
{
    internal sealed class RefreshCommandHandler : IRequestHandler<RefreshCommand, OneOf<bool, Error>>
    {
        private readonly IRefreshRepository _refreshRepository;
        private readonly ITokenProvider _tokenProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public RefreshCommandHandler(IRefreshRepository refreshRepository, ITokenProvider tokenProvider, IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager)
        {
            _refreshRepository = refreshRepository;
            _tokenProvider = tokenProvider;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public async Task<OneOf<bool, Error>> Handle(RefreshCommand request, CancellationToken cancellationToken)
        {
            var refreshToken = await _refreshRepository.GetRefreshTokenAsync(request.RefreshToken, cancellationToken);
            if (refreshToken is null || !refreshToken.IsActive)
            {
                return UserErrors.Unauthorized;
            }
            var user = await _userManager.FindByIdAsync(refreshToken.UserId.ToString());
            if (user is null)
            {
                return UserErrors.Unauthorized;
            }
            refreshToken.RevokedAt = DateTime.UtcNow;
            _tokenProvider.GenerateJwtToken(user);
            var newRefreshToken = _tokenProvider.GenerateRefreshToken();
            _refreshRepository.AddRefreshToken(user, newRefreshToken.token, newRefreshToken.expiresOn, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            return true;
        }
    }
}
