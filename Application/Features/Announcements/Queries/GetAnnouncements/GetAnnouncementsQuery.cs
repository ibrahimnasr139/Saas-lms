using Application.Features.Announcements.Dtos;

namespace Application.Features.Announcements.Queries.GetAnnouncements
{
    public sealed record GetAnnouncementsQuery(int Limit, string? Q, int Cursor) : IRequest<AllAnnouncements>;
}