using Application.Contracts.Externals;
using Application.Features.StudentLessons.Dtos;
using Domain.Enums;
using Infrastructure.Common.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Application.Features.StudentLessons.Commands.AiChatMessage
{
    internal sealed class AiChatMessageCommandHandler : IRequestHandler<AiChatMessageCommand, OneOf<AiChatMessageDto, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IStudentSubscriptionRepository _studentSubscriptionRepository;
        private readonly IModuleItemRepository _moduleItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICourseRepository _courseRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly IModuleRepository _moduleRepository;
        private readonly IExternalService _externalService;
        private readonly AiOptions _options;

        public AiChatMessageCommandHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor,
            IEnrollmentRepository enrollmentRepository, IStudentSubscriptionRepository studentSubscriptionRepository,
            IModuleItemRepository moduleItemRepository, IUnitOfWork unitOfWork, ICourseRepository courseRepository,
            ILessonRepository lessonRepository, IModuleRepository moduleRepository, IOptions<AiOptions> options,
            IExternalService externalService)
        {
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _enrollmentRepository = enrollmentRepository;
            _studentSubscriptionRepository = studentSubscriptionRepository;
            _moduleItemRepository = moduleItemRepository;
            _unitOfWork = unitOfWork;
            _courseRepository = courseRepository;
            _lessonRepository = lessonRepository;
            _moduleRepository = moduleRepository;
            _externalService = externalService;
            _options = options.Value;
        }
        public async Task<OneOf<AiChatMessageDto, Error>> Handle(AiChatMessageCommand request, CancellationToken cancellationToken)
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

            var isEnrolled = await _enrollmentRepository.StudentIsAlreadyEnrolledAsync(session.StudentId, request.CourseId, cancellationToken);
            if (!isEnrolled)
                return StudentCourseErrors.StudentNotEnrolledInCourse;

            var subscriptionIsActive = await _studentSubscriptionRepository.StudentSubscriptionIsActiveAsync(session.StudentId, request.CourseId, cancellationToken);
            if (!subscriptionIsActive)
                return StudentSubscriptionErrors.StudentSubscribedExpired;

            var moduleItemIsExist = await _moduleItemRepository.ModuleItemIsExistAsync(request.ItemId, request.CourseId, cancellationToken);
            if (!moduleItemIsExist)
                return ModuleItemErrors.ModuleItemNotFound;

            var courseName = await _courseRepository.GetCourseNameAsync(request.CourseId, cancellationToken);
            var lessonName = await _lessonRepository.GetLessonNameAsync(request.ItemId, cancellationToken);
            var moduleName = await _moduleRepository.GetModuleNameAsync(request.ItemId, request.CourseId, cancellationToken);
            var fileId = await _lessonRepository.GetVideoIdAsync(request.ItemId, request.CourseId, cancellationToken);

            var payload = new AiAssistantRequest
            {
                Message = request.Message,
                Course = courseName,
                Module = moduleName!,
                FileId = fileId,
                Lesson = lessonName
            };

            var endpoint = _options.AiAssistantEndPoint;
            var result = await _externalService
                .CallExternalServiceAsync<AiAssistantRequest, AiAssistantResponse>(endpoint, payload, cancellationToken);
            if (result is null)
                throw new Exception();

            List<AiAssistantMessage> messages = new()
            {
                new AiAssistantMessage
                {
                    Content = request.Message,
                    Role = RoleType.User,
                    StudentId = session.StudentId,
                    LessonId = request.ItemId
                },
                new AiAssistantMessage
                {
                    Content = result.Response,
                    Role = RoleType.Assistant,
                    StudentId = session.StudentId,
                    LessonId = request.ItemId
                }
            };
            await _lessonRepository.CreateAiAssistantMessageAsync(messages, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            return new AiChatMessageDto { Answer = result.Response, Message = request.Message };
        }
    }
}