using Application.Features.StudentUsers.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.StudentUsers.Queries.GetCurrentStudent
{
    internal sealed class GetCurrentStudentQueryHandler : IRequestHandler<GetCurrentStudentQuery, OneOf<CurrentStudentDto, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStudentStreakRepository _studentStreakRepository;

        public GetCurrentStudentQueryHandler(HybridCache hybridCache, IStudentRepository studentRepository,IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor, IStudentStreakRepository studentStreakRepository)
        {
            _hybridCache = hybridCache;
            _studentRepository = studentRepository;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _studentStreakRepository = studentStreakRepository;
        }
        public async Task<OneOf<CurrentStudentDto, Error>> Handle(GetCurrentStudentQuery request, CancellationToken cancellationToken)
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

            var streakUpdated = await _studentStreakRepository.UpdateStudentStreakAsync(session.StudentId, cancellationToken);
            if (streakUpdated)
                await _unitOfWork.SaveAsync(cancellationToken);
            return await _studentRepository.GetCurrentStudentAsync(session.UserId, session.StudentId, cancellationToken);
        }
    }
}