using Application.Contracts.Zoom;
using Application.Features.Tenants.Dtos;
using Application.Helpers;
using Domain.Enums;
using Hangfire;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Tenants.Commands.DeleteLiveSession
{
    internal sealed class DeleteLiveSessionCommandHandler : IRequestHandler<DeleteLiveSessionCommand, OneOf<DeleteLiveSessionDto, Error>>
    {
        private readonly ICurrentUserId _currentUserId;
        private readonly ILiveSessionRepository _liveSessionRepository;
        private readonly IZoomService _zoomService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICourseRepository _courseRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IEmailSender _emailSender;
        private readonly ITenantMemberRepository _tenantMemberRepository;

        public DeleteLiveSessionCommandHandler(ICurrentUserId currentUserId, ILiveSessionRepository liveSessionRepository,
            IZoomService zoomService, IHttpContextAccessor httpContextAccessor,
            ICourseRepository courseRepository, ITenantRepository tenantRepository, IEmailSender emailSender, ITenantMemberRepository tenantMemberRepository)
        {
            _currentUserId = currentUserId;
            _liveSessionRepository = liveSessionRepository;
            _zoomService = zoomService;
            _httpContextAccessor = httpContextAccessor;
            _courseRepository = courseRepository;
            _tenantRepository = tenantRepository;
            _emailSender = emailSender;
            _tenantMemberRepository = tenantMemberRepository;
        }
        public async Task<OneOf<DeleteLiveSessionDto, Error>> Handle(DeleteLiveSessionCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var isPermitted = await _tenantMemberRepository.IsPermittedMember(userId, PermissionConstants.EDIT_SESSIONS, cancellationToken);
            if (!isPermitted)
                return MemberErrors.NotAllowed;

            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);
            var tenant = await _tenantRepository.GetLastTenantAsync(subDomain, cancellationToken);
            var session = await _liveSessionRepository.GetLiveSessionAsync(request.SessionId, cancellationToken);
            if (session == null)
                return LiveSessionErrors.SessionNotFound;

            if (session.Host.UserId != userId)
                return LiveSessionErrors.CannotDeleteOthersLiveSession;

            if (session.Status == LiveSessionStatus.Ongoing)
                return LiveSessionErrors.CannotDeleteActiveLiveSession;

            if (session.ZoomIntegration == null || !session.ZoomIntegration.IsActive)
                return ZoomError.ZoomAccountNotConnected;

            if (session.ZoomIntegration.TokenExpiresAt <= DateTime.UtcNow)
            {
                var refreshed = await _zoomService.RefreshZoomTokenAsync(session.ZoomIntegration, cancellationToken);
                if (!refreshed)
                    return ZoomError.ZoomTokenRefreshFailed;
            }

            var course = await _courseRepository.GetCourseByIdAsync(session.CourseId, subDomain!, cancellationToken);
            await _liveSessionRepository.DeleteAsync(request.SessionId, cancellationToken);
            await _zoomService.DeleteZoomMeetingAsync(session.ZoomIntegration.AccessToken, session.ZoomMeetingId, cancellationToken);

            if (course != null && session.Status != LiveSessionStatus.Completed)
            {
                foreach (var enrollment in course.Enrollments)
                {
                    var student = enrollment.Student;
                    var emailBody = EmailConfirmationHelper.GenerateEmailBodyHelper(
                        EmailConstants.CancelLiveSessionTemplate,
                        new Dictionary<string, string>
                        {
                            { "{{student_name}}", $"{student.User.FirstName} {student.User.LastName}" },
                            { "{{course_name}}", course.Title },
                            { "{{session_title}}", session.Title },
                            { "{{instructor_name}}", session.ZoomIntegration.ZoomDisplayName },
                            { "{{session_date}}", session.ScheduledAt.ToString("yyyy-MM-dd") },
                            { "{{session_time}}", session.ScheduledAt.ToString("HH:mm") },
                            { "{{platform_name}}", tenant?.PlatformName ?? "Platform" }
                        });
                    BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(
                        student.User.Email!,
                        $"{EmailConstants.DeleteSubject}: {session.Title}",
                        emailBody));
                }
            }
            return new DeleteLiveSessionDto { Message = MessagesConstants.LiveSessionDeleted };
        }
    }
}