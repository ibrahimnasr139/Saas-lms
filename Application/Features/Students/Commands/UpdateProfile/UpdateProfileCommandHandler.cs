using Microsoft.AspNetCore.Http;

namespace Application.Features.Students.Commands.UpdateProfile
{
    internal sealed class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, OneOf<bool, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStudentRepository _studentRepository;
        public UpdateProfileCommandHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor,
           IStudentRepository studentRepository, IUnitOfWork unitOfWork)
        {
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _studentRepository = studentRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<bool, Error>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var sessionId = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SessionId];
            var cachedSessionKey = $"{CacheKeysConstants.SessionKey}_{sessionId}";
            var session = await _hybridCache.GetOrCreateAsync<UserSession?>(
                cachedSessionKey,
                _ => ValueTask.FromResult<UserSession?>(null),
                cancellationToken: cancellationToken
            );
            if (session is null)
                return UserErrors.Unauthorized;

            var result = await _studentRepository.UpdateUserProfileAsync(session.StudentId, request, cancellationToken);
            if (!result)
                return StudentErrors.StudentNotFound;
            await _unitOfWork.SaveAsync(cancellationToken);
            return true;
        }
    }
}