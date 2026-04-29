using Application.Features.Announcements.Dtos;

namespace Application.Contracts.Repositories
{
    public interface IAnnouncementRepository
    {
        Task CreateAnnouncementAsync(Announcement announcement, CancellationToken cancellationToken);
        Task<bool> DeleteAnnouncementAsync(int announcementId, string subDomain, CancellationToken cancellationToken);
        Task<AllAnnouncements> GetAnnouncementsAsync(string subDomain, int Limit, string? Q, int Cursor, CancellationToken cancellationToken);
    }
}