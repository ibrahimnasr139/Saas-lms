using Application.Features.Announcements.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Announcements.Commands.DeleteAnnouncement
{
    internal sealed class DeleteAnnouncementCommandHandler : IRequestHandler<DeleteAnnouncementCommand, OneOf<AnnouncementResponse, Error>>
    {
        private readonly IAnnouncementRepository _announcementRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteAnnouncementCommandHandler(IAnnouncementRepository announcementRepository, IHttpContextAccessor httpContextAccessor)
        {
            _announcementRepository = announcementRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<OneOf<AnnouncementResponse, Error>> Handle(DeleteAnnouncementCommand request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var result = await _announcementRepository.DeleteAnnouncementAsync(request.AnnouncementId, subDomain!, cancellationToken);
            if (!result)
                return AnnouncementErrors.AnnouncementNotFound;
            return new AnnouncementResponse { Messsage = MessagesConstants.AnnouncementDeleted };
        }
    }
}