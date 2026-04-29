using Application.Contracts.Zoom;
using Application.Features.Tenants.Dtos;
using Application.Helpers;
using Domain.Enums;
using Hangfire;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Tenants.Commands.CreateLiveSession
{
    internal sealed class CreateLiveSessionCommandHandler : IRequestHandler<CreateLiveSessionCommand, OneOf<CreateLiveSessionDto, Error>>
    {
        private readonly IZoomService _zoomService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IZoomIntegrationRepository _zoomIntegrationRepository;
        private readonly ILiveSessionRepository _liveSessionRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly IEmailSender _emailSender;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateLiveSessionCommandHandler(IZoomService zoomService, IHttpContextAccessor httpContextAccessor,
            IZoomIntegrationRepository zoomIntegrationRepository, ILiveSessionRepository liveSessionRepository, IUnitOfWork unitOfWork,
            ITenantRepository tenantRepository, ICourseRepository courseRepository, ICurrentUserId currentUserId, 
            ITenantMemberRepository tenantMemberRepository, IEmailSender emailSender, ISubscriptionRepository subscriptionRepository)
        {
            _zoomService = zoomService;
            _httpContextAccessor = httpContextAccessor;
            _zoomIntegrationRepository = zoomIntegrationRepository;
            _liveSessionRepository = liveSessionRepository;
            _tenantRepository = tenantRepository;
            _courseRepository = courseRepository;
            _currentUserId = currentUserId;
            _tenantMemberRepository = tenantMemberRepository;
            _emailSender = emailSender;
            _subscriptionRepository = subscriptionRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<CreateLiveSessionDto, Error>> Handle(CreateLiveSessionCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var isPermitted = await _tenantMemberRepository.IsPermittedMember(userId, PermissionConstants.CREATE_SESSIONS, cancellationToken);
            if (!isPermitted)
                return MemberErrors.NotAllowed;

            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);
            var tenant = await _tenantRepository.GetLastTenantAsync(subDomain, cancellationToken);
            var tenantMemberId = await _tenantMemberRepository.GetMemberIdByUserIdAsync(userId, tenantId, cancellationToken);

            var hasFeature = await _subscriptionRepository.TenantHasFeatureAsync(tenantId, LiveSessionConstants.LiveSessionFeatureKey, cancellationToken);
            if (!hasFeature)
                return LiveSessionErrors.ZoomIntegrationNotAvailable;

            var course = await _courseRepository.GetCourseByIdAsync(request.CourseId, subDomain!, cancellationToken);
            if (course == null)
                return CourseErrors.CourseNotFound;

            var zoomIntegration = await _zoomIntegrationRepository.GetZoomIntegrationAsync(userId, tenantId, cancellationToken);
            if (zoomIntegration == null || !zoomIntegration.IsActive)
                return ZoomError.ZoomAccountNotConnected;

            if (zoomIntegration.TokenExpiresAt <= DateTime.UtcNow)
            {
                var refreshed = await _zoomService.RefreshZoomTokenAsync(zoomIntegration, cancellationToken);
                if (!refreshed)
                    return ZoomError.ZoomTokenRefreshFailed;
            }

            var zoomMeeting = await _zoomService.CreateZoomMeetingAsync(zoomIntegration.AccessToken, request, cancellationToken);
            if (zoomMeeting == null)
                return ZoomError.ZoomMeetingCreationFailed;

            var session = new LiveSession
            {
                Title = request.Title,
                Description = request.Description,
                Duration = request.Duration,
                ZoomMeetingId = zoomMeeting.id.ToString(),
                ZoomHostId = zoomMeeting.host_id,
                ZoomJoinUrl = zoomMeeting.join_url,
                ZoomStartUrl = zoomMeeting.start_url,
                ZoomHostEmail = zoomMeeting.host_email,
                ZoomPassword = zoomMeeting.password,
                Status = LiveSessionStatus.Upcoming,
                ScheduledAt = request.ScheduledAt,
                ActualStartTime = request.ScheduledAt,
                TenantId = tenantId,
                CourseId = request.CourseId,
                HostMemberId = tenantMemberId,
                ZoomIntegrationId = zoomIntegration.Id,
                EnableChat = request.Settings.EnableChat,
                ParticipantVideo = request.Settings.ParticipantVideo,
                WaitingRoom = request.Settings.WaitingRoom
            };
            await _liveSessionRepository.CreateAsync(session, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);

            zoomIntegration.LastSyncAt = DateTime.UtcNow;
            await _unitOfWork.SaveAsync(cancellationToken);

            if (request.Notifications.SendEmail)
            {
                foreach (var enrollment in course.Enrollments)
                {
                    var student = enrollment.Student;
                    var emailBody = EmailConfirmationHelper.GenerateEmailBodyHelper(
                        EmailConstants.LiveSessionTemplate,
                        new Dictionary<string, string>
                        {
                            { "{{student_name}}", student.User.FirstName + " " + student.User.LastName },
                            { "{{session_title}}", session.Title },
                            { "{{session_description}}", session.Description ?? "" },
                            { "{{instructor_name}}", zoomIntegration.ZoomDisplayName },
                            { "{{course_name}}", course.Title },
                            { "{{session_date}}", session.ScheduledAt.ToString("yyyy-MM-dd") },
                            { "{{session_time}}", session.ScheduledAt.ToString("HH:mm") },
                            { "{{session_duration}}", session.Duration.ToString() },
                            { "{{zoom_meeting_id}}", session.ZoomMeetingId },
                            { "{{zoom_password}}", session.ZoomPassword ?? "" },
                            { "{{join_url}}", session.ZoomJoinUrl },
                            { "{{platform_name}}", tenant!.PlatformName }
                        });
                    BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(student.User.Email!, EmailConstants.Subject, emailBody));
                }
            }
            return new CreateLiveSessionDto { Message = MessagesConstants.LiveSessionCreated };
        }
    }
}