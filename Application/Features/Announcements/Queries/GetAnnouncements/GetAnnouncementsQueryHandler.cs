using Application.Features.Announcements.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Announcements.Queries.GetAnnouncements
{
    internal sealed class GetAnnouncementsQueryHandler : IRequestHandler<GetAnnouncementsQuery, AllAnnouncements>
    {
        private readonly IAnnouncementRepository _announcementRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetAnnouncementsQueryHandler(IAnnouncementRepository announcementRepository, IHttpContextAccessor httpContextAccessor)
        {
            _announcementRepository = announcementRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<AllAnnouncements> Handle(GetAnnouncementsQuery request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            return await _announcementRepository.GetAnnouncementsAsync(subDomain!, request.Limit, request.Q, request.Cursor, cancellationToken);
        }
    }
}