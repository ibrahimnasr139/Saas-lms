using Application.Features.Announcements.Dtos;
using Application.Helpers;
using Hangfire;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Announcements.Commands.CreateAnnouncement
{
    internal sealed class CreateAnnouncementCommandHandler : IRequestHandler<CreateAnnouncementCommand, AnnouncementResponse>
    {
        private readonly IAnnouncementRepository _announcementRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantRepository _tenantRepository;
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork _unitOfWork;

        public CreateAnnouncementCommandHandler(IAnnouncementRepository announcementRepository, IHttpContextAccessor httpContextAccessor,
            ITenantRepository tenantRepository, ITenantMemberRepository tenantMemberRepository, ICurrentUserId currentUserId,
            IEnrollmentRepository enrollmentRepository, IEmailSender emailSender, IUnitOfWork unitOfWork)
        {
            _announcementRepository = announcementRepository;
            _httpContextAccessor = httpContextAccessor;
            _tenantRepository = tenantRepository;
            _tenantMemberRepository = tenantMemberRepository;
            _currentUserId = currentUserId;
            _enrollmentRepository = enrollmentRepository;
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
        }
        public async Task<AnnouncementResponse> Handle(CreateAnnouncementCommand request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var userId = _currentUserId.GetUserId();
            var tenant = await _tenantRepository.GetLastTenantAsync(subDomain!, cancellationToken);
            var createdBy = await _tenantMemberRepository.GetMemberIdByUserIdAsync(userId, tenant!.Id, cancellationToken);
            var member = await _tenantMemberRepository.GetMemberByIdAsync(createdBy, cancellationToken);

            var announcement = new Announcement
            {
                Title = request.Title,
                Content = request.Content,
                IsPinned = request.IsPinned,
                TargetType = request.TargetType,
                TargetCourseIds = request.TargetCourseIds,
                CreatedBy = createdBy,
                TenantId = tenant!.Id,
            };

            await _announcementRepository.CreateAnnouncementAsync(announcement, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);

            var studentEmails = request.TargetCourseIds is not null
                ? await _enrollmentRepository.GetEmailsByCourseIdsAsync(request.TargetCourseIds, cancellationToken)
                : await _enrollmentRepository.GetAllStudentEmailsAsync(tenant.Id, cancellationToken);

            var emailBody = EmailConfirmationHelper.GenerateEmailBodyHelper(
                EmailConstants.AnnouncementTemplate,
                new Dictionary<string, string>
                {
                    { "{{announcer_name}}", member!.DisplayName! },
                    { "{{platform_name}}", tenant!.PlatformName },
                    { "{{title}}", announcement.Title },
                    { "{{content}}", announcement.Content },
                    { "{{created_at}}", announcement.CreatedAt.ToString("yyyy/MM/dd") },
                    { "{{year}}", DateTime.UtcNow.Year.ToString() }
                });

            foreach (var email in studentEmails)
                BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(email, EmailConstants.AnnouncementSubject, emailBody));
            return new AnnouncementResponse { Messsage = MessagesConstants.AnnouncementCreated };
        }
    }
}