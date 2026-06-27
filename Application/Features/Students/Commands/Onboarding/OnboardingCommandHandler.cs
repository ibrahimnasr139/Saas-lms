using Application.Features.TenantStudents.Dtos;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Application.Features.Students.Commands.Onboarding
{
    internal sealed class OnboardingCommandHandler : IRequestHandler<OnboardingCommand, OneOf<StudentResponse, Error>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly HybridCache _hybridCache;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStudentSubjectRepository _studentSubjectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStudentStreakRepository _studentStreakRepository;

        public OnboardingCommandHandler(IStudentRepository studentRepository, HybridCache hybridCache, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, IStudentSubjectRepository studentSubjectRepository, IUnitOfWork unitOfWork,
            IStudentStreakRepository studentStreakRepository)
        {
            _studentRepository = studentRepository;
            _hybridCache = hybridCache;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _studentSubjectRepository = studentSubjectRepository;
            _unitOfWork = unitOfWork;
            _studentStreakRepository = studentStreakRepository;
        }
        public async Task<OneOf<StudentResponse, Error>> Handle(OnboardingCommand request, CancellationToken cancellationToken)
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

            var student = await _studentRepository.GetStudentAsync(session.StudentId, cancellationToken);
            if (student is null)
                return UserErrors.Unauthorized;

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                _mapper.Map(request, student);
                await _studentRepository.UpdateHasOnboardedAsync(session.UserId, cancellationToken);

                var subjectIds = await _studentSubjectRepository.GetSubjectIdsAsync(request.Subjects, cancellationToken);
                var missingSubjects = request.Subjects.Except(subjectIds.Keys).ToList();
                if (missingSubjects.Any())
                    return new Error("InvalidSubjects", $"لم يتم العثور على المواد التالية: {string.Join("، ", missingSubjects)}", HttpStatusCode.BadRequest);

                var newStudentSubjects = subjectIds.Select(kvp => new StudentSubject
                {
                    Confidence = request.Confidence[kvp.Key],
                    StudentId = session.StudentId,
                    AvailableSubjectId = kvp.Value
                }).ToList();

                var newStudentStreak = new StudentStreak { StudentId = session.StudentId };

                await _studentSubjectRepository.CreateStudentSubjectAsync(newStudentSubjects, cancellationToken);
                await _studentStreakRepository.CreateStudentStreakAsync(newStudentStreak, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return new StudentResponse { Message = MessagesConstants.StudentOnboardingCompleted };
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}